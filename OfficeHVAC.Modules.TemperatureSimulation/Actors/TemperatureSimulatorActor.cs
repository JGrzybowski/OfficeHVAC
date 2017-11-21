using Akka.Actor;
using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using System;
using System.Linq;
using OfficeHVAC.Models.Actors;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureSimulatorActor : DebuggableActor<TemperatureSimulatorActor.Status>
    {
        private IRoomStatusMessage roomStatus;
        private readonly ITemperatureSimulator temperatureSimulator;

        private readonly IActorRef subsciptionManager;
        
        private Duration Threshold { get; set; } = Duration.FromSeconds(5);
        private Duration ThresholdBuffer { get; set; } = Duration.Zero;
        private Instant LastSubscriptionTimestamp { get; set; } = Instant.MinValue;

        private Instant lastTimestamp = Instant.MinValue;
        private Instant LastTimestamp
        {
            get { return lastTimestamp; }
            set
            {
                lastTimestamp = value; 
                InformAboutInternalState();
            }
        }

        private bool receivedInitialTimestamp;
        private bool receivedInitialTemperatureModel;
        private bool receivedInitialStatus;
        private bool ReceivedInitialData => receivedInitialTimestamp && receivedInitialTemperatureModel && receivedInitialStatus;
        
        public TemperatureSimulatorActor(ITemperatureSimulator temperatureSimulator, string temperatureParamsActorPath)
        {
            this.temperatureSimulator = temperatureSimulator;
            subsciptionManager = Context.ActorOf<SubscriptionActor>();
            
            Become(AwaitingInitialModel);

            ICanTell temperatureParamsActorContact = Context.System.ActorSelection(temperatureParamsActorPath);
            temperatureParamsActorContact.Tell(new SubscribeMessage(Self), Self);
        }

        private void AwaitingInitialModel()
        {
            Receive<ITemperatureModel>(model =>
            {
                temperatureSimulator.ReplaceTemperatureModel(model);
                receivedInitialTemperatureModel = true;
                if(ReceivedInitialData)
                    Become(Processing);
            });

            Receive<TimeChangedMessage>(msg =>
            {
                LastTimestamp = msg.Now;
                receivedInitialTimestamp = true;
                if (ReceivedInitialData) 
                    Become(Processing);
            });

            Receive<IRoomStatusMessage>(msg =>
            {
                receivedInitialStatus = true;
                UpdateStatus(msg);
                SetTemperature((double) msg.Parameters[SensorType.Temperature].Value);
                if (ReceivedInitialData) 
                    Become(Processing);
            });
            
            Receive<SubscribeMessage>(message => subsciptionManager.Forward(message));
            Receive<UnsubscribeMessage>(message => subsciptionManager.Forward(message));
            RegisterDebugSubscribers();
        }

        private void Processing()
        {
            Receive<ParameterValue.Request>(
                msg => Sender.Tell(new ParameterValue(SensorType.Temperature, temperatureSimulator.Temperature)),
                msg => msg.ParameterType == SensorType.Temperature
            );

            Receive<TimeChangedMessage>(msg => AddTimeToBuffer(msg));

            Receive<IRoomStatusMessage>(msg => UpdateStatus(msg));
            
            Receive<ITemperatureModel>(model => temperatureSimulator.ReplaceTemperatureModel(model));
            
            Receive<SubscribeMessage>(message => subsciptionManager.Forward(message));
            Receive<UnsubscribeMessage>(message => subsciptionManager.Forward(message));
            RegisterDebugSubscribers();
        }

        protected void AddTimeToBuffer(TimeChangedMessage msg)
        {                    
            var timeDiff = msg.Now - LastTimestamp;
            LastTimestamp = msg.Now;

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
                temperatureSimulator.Temperature,
                this.ThresholdBuffer,
                this.LastTimestamp
            );
        }
        
        public static Props Props(RoomStatus initialStatus, string timeActorPath, string tempParamsActorPath)
        {
            double initialTemperature = 0;
            if (initialStatus.Parameters.Contains(SensorType.Temperature))
                initialTemperature = Convert.ToDouble(initialStatus.Parameters[SensorType.Temperature].Value);

            var props = Akka.Actor.Props.Create(
                () => new TemperatureSimulatorActor(new TemperatureSimulator(initialTemperature, null), tempParamsActorPath)
            );

            return props;
        }
        
        public class Status
        {
            public Status(double temperature, Duration theresholdBuffer, Instant lastTimestamp)
            {
                Temperature = temperature;
                TheresholdBuffer = theresholdBuffer;
                LastTimestamp = lastTimestamp;
            }
    
            public double Temperature { get; }
            public Duration TheresholdBuffer { get; }
            public Instant LastTimestamp { get; }
        }
    }
}