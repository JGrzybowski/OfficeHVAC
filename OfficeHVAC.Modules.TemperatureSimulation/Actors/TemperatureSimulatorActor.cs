using Akka.Actor;
using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using System;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureSimulatorActor : ReceiveActor
    {
        private IRoomStatusMessage roomStatus;

        private ICanTell temperatureaParamsActorContact { get; set; }
        
        private IActorRef SubsciptionManager { get; set; }
        
        private ITemperatureSimulator temperatureSimulator { get; }
        
        private Duration Threshold { get; set; } = Duration.FromSeconds(5);
        private Duration ThresholdBuffer { get; set; } = Duration.Zero;
        private Instant LastSubscriptionTimestamp { get; set; } = Instant.MinValue;
        private Instant LastTimestamp { get; set; } = Instant.MinValue;

        private bool receivedInitialTimestamp = false;
        private bool receivedInitialTemperatureModel = false;
        private bool ReceivedInitialData => receivedInitialTimestamp && receivedInitialTemperatureModel;
        
        public TemperatureSimulatorActor(ITemperatureSimulator temperatureSimulator, string temperatureParamsActorPath)
        {
            this.temperatureSimulator = temperatureSimulator;

            temperatureaParamsActorContact = Context.System.ActorSelection(temperatureParamsActorPath);

            SubsciptionManager = Context.ActorOf<SubscriptionActor>();
            
            Become(AwaitingInitialModel);
            temperatureaParamsActorContact.Tell(new SubscribeMessage(Self), Self);
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

            Receive<TimeChangedMessage>( 
                msg =>
                {
                    LastTimestamp = msg.Now;
                    receivedInitialTimestamp = true;
                    if (ReceivedInitialData) 
                        Become(Processing);
                });
            
            Receive<SubscribeMessage>(message => SubsciptionManager.Forward(message));
            Receive<UnsubscribeMessage>(message => SubsciptionManager.Forward(message));
        }

        private void Processing()
        {
            Receive<ParameterValue.Request>(
                msg => Sender.Tell(new ParameterValue(SensorType.Temperature, temperatureSimulator.Temperature)),
                msg => msg.ParameterType == SensorType.Temperature
            );

            Receive<TimeChangedMessage>(msg => AddTimeToBuffer(msg));

            Receive<IRoomStatusMessage>(msg => roomStatus = msg);
            
            Receive<ITemperatureModel>(model => temperatureSimulator.ReplaceTemperatureModel(model));
            
            Receive<SubscribeMessage>(message => SubsciptionManager.Forward(message));
            Receive<UnsubscribeMessage>(message => SubsciptionManager.Forward(message));
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
                SubsciptionManager.Tell(new SendToSubscribersMessage(statusUpdateMessage));
                ThresholdBuffer = Duration.Zero;
            }
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
    }
}