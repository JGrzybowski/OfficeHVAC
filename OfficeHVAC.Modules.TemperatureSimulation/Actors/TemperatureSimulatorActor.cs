using Akka.Actor;
using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using OfficeHVAC.Components;
using OfficeHVAC.Models.Actors;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureSimulatorActor : SimulatingComponentActor<TemperatureSimulatorActorStatus, double>
    {
        private readonly ITemperatureSimulator temperatureSimulator;

        private bool receivedInitialTemperatureModel;

        protected override bool ReceivedInitialData() =>
            TimeStampInitialized && receivedInitialTemperatureModel && ReceivedInitialRoomStatus;

        public TemperatureSimulatorActor(ITemperatureSimulator temperatureSimulator,
            IEnumerable<string> subscriptionSources) : base(subscriptionSources)
        {
            this.temperatureSimulator = temperatureSimulator;
            Become(Uninitialized);
        }

        protected override void Uninitialized()
        {
            Receive<ITemperatureModel>(model =>
            {
                temperatureSimulator.ReplaceTemperatureModel(model);
                receivedInitialTemperatureModel = true;
                if (ReceivedInitialData())
                    Become(Initialized);
            });

            base.Uninitialized(); 
        }

        protected override void Initialized()
        {
            Receive<ParameterValue.Request>(
                msg => Sender.Tell(new ParameterValue(SensorType.Temperature, temperatureSimulator.Temperature)),
                msg => msg.ParameterType == SensorType.Temperature
            );

            Receive<ITemperatureModel>(model => temperatureSimulator.ReplaceTemperatureModel(model));

            base.Initialized();
        }

        protected override void OnTimeUpdated(Duration timeDiff) =>
            temperatureSimulator.ChangeTemperature(RoomStatus, timeDiff);

        protected override void OnThresholdCrossed()
        {
            var statusUpdateMessage = new ParameterValue(SensorType.Temperature, temperatureSimulator.Temperature);
            SubsciptionManager.Tell(new SendToSubscribersMessage(statusUpdateMessage));
            base.OnThresholdCrossed();
        }

        protected override void InitializeFromRoomStatus(IRoomStatusMessage roomStatus)
        {
            if (!roomStatus.Parameters.Contains(SensorType.Temperature))
                return;
            
            SetParameterValue((double) roomStatus.Parameters[SensorType.Temperature].Value);
            base.InitializeFromRoomStatus(roomStatus);
        }

        protected override void SetParameterValue(double value)
        {
            temperatureSimulator.Temperature = value;
            SubsciptionManager.Tell(new ParameterValue(SensorType.Temperature, value));
        }

        protected override TemperatureSimulatorActorStatus GenerateInternalState()
        {
            return new TemperatureSimulatorActorStatus
            (
                Id,
                temperatureSimulator.Temperature,
                ThresholdBuffer,
                Timestamp
            );
        }

        public static Props Props(RoomStatus initialStatus, string timeActorPath, string tempParamsActorPath)
        {
            double initialTemperature = 0;
            if (initialStatus.Parameters.Contains(SensorType.Temperature))
                initialTemperature = Convert.ToDouble(initialStatus.Parameters[SensorType.Temperature].Value);

            var props = Akka.Actor.Props.Create(
                () => new TemperatureSimulatorActor(new TemperatureSimulator(initialTemperature, null),
                    new[] {tempParamsActorPath, timeActorPath})
            );

            return props;
        }
    }

    public class TemperatureSimulatorActorStatus : ComponentStatus<double>
    {
            public TemperatureSimulatorActorStatus(string id, double temperature, Duration theresholdBuffer, Instant timestamp) 
                : base(id, temperature, timestamp)
            {
                TheresholdBuffer = theresholdBuffer;
            }
    
            public Duration TheresholdBuffer { get; }
    }
}