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
    public class TemperatureSimulatorActor : ComponentActor<TemperatureSimulatorActor.Status, double>
    {
        private IRoomStatusMessage roomStatus;
        private readonly ITemperatureSimulator temperatureSimulator;

        private readonly IActorRef subsciptionManager;
        
        private Duration Threshold { get; set; } = Duration.FromSeconds(5);
        private Duration ThresholdBuffer { get; set; } = Duration.Zero;

        private bool receivedInitialTimestamp;
        private bool receivedInitialTemperatureModel;
        private bool receivedInitialStatus;
        protected override bool ReceivedInitialData() => receivedInitialTimestamp && receivedInitialTemperatureModel && receivedInitialStatus;
        
        public TemperatureSimulatorActor(ITemperatureSimulator temperatureSimulator, IEnumerable<string> subscribionSources) : base(subscribionSources)
        {
            this.temperatureSimulator = temperatureSimulator;
            subsciptionManager = Context.ActorOf<SubscriptionActor>();
            
            Become(Uninitialized);
        }

        protected override void Uninitialized()
        {
            Receive<ITemperatureModel>(model =>
            {
                temperatureSimulator.ReplaceTemperatureModel(model);
                receivedInitialTemperatureModel = true;
                if(ReceivedInitialData())
                    Become(Initialized);
            });

            Receive<IRoomStatusMessage>(msg =>
            {
                receivedInitialStatus = true;
                UpdateStatus(msg);
                SetTemperature((double) msg.Parameters[SensorType.Temperature].Value);
                if (ReceivedInitialData()) 
                    Become(Initialized);
            });
            
            Receive<SubscribeMessage>(message => subsciptionManager.Forward(message));
            Receive<UnsubscribeMessage>(message => subsciptionManager.Forward(message));
            base.Uninitialized();
        }

        protected override void Initialized()
        {
            Receive<ParameterValue.Request>(
                msg => Sender.Tell(new ParameterValue(SensorType.Temperature, temperatureSimulator.Temperature)),
                msg => msg.ParameterType == SensorType.Temperature
            );

            Receive<IRoomStatusMessage>(msg => UpdateStatus(msg));
         
            Receive<ITemperatureModel>(model => temperatureSimulator.ReplaceTemperatureModel(model));
            
            Receive<SubscribeMessage>(message => subsciptionManager.Forward(message));
            Receive<UnsubscribeMessage>(message => subsciptionManager.Forward(message));
            base.Initialized();
        }

        protected override void OnTimeChangedMessage(TimeChangedMessage msg)
        {                    
            var timeDiff = msg.Now - Timestamp;
            Timestamp = msg.Now;

            temperatureSimulator.ChangeTemperature(roomStatus, timeDiff);
            ThresholdBuffer += timeDiff;
            
            if (ThresholdBuffer > Threshold)
            {
                var statusUpdateMessage = new ParameterValue(SensorType.Temperature, temperatureSimulator.Temperature);
                ThresholdBuffer = Duration.Zero;
                InformAboutInternalState();
                subsciptionManager.Tell(new SendToSubscribersMessage(statusUpdateMessage));
            }
        }

        private void UpdateStatus(IRoomStatusMessage status)
        {
            roomStatus = status;
            var temp = status.Parameters.SingleOrDefault(p => p.ParameterType == SensorType.Temperature);

            subsciptionManager.Tell(new ParameterValue(SensorType.Temperature, temp));
        }

        private void SetTemperature(double value)
        {
            temperatureSimulator.Temperature = value;
            subsciptionManager.Tell(new ParameterValue(SensorType.Temperature, value));
        }
        
        protected override Status GenerateInternalState()
        {
            return new Status
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
                () => new TemperatureSimulatorActor(new TemperatureSimulator(initialTemperature, null), new []{tempParamsActorPath, timeActorPath})
            );

            return props;
        }
        
        public class Status : ComponentStatus<double>
        {
            public Status(string id, double temperature, Duration theresholdBuffer, Instant timestamp) : base(id, temperature, timestamp)
            {
                Temperature = temperature;
                TheresholdBuffer = theresholdBuffer;
                Timestamp = timestamp;
            }
    
            public double Temperature { get; }
            public Duration TheresholdBuffer { get; }
            public Instant Timestamp { get; }
        }
    }
}