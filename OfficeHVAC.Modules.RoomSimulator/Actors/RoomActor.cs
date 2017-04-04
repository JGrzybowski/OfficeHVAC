using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using System.Collections.Generic;
using System.Linq;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomActor : ReceiveActor
    {
        protected RoomStatus Status { get; } = new RoomStatus();

        protected ActorPath CompanySupervisorActorPath { get; }

        protected HashSet<ISensorActorRef> Sensors { get; } = new HashSet<ISensorActorRef>();

        protected HashSet<ISensorActorRef> Controllers { get; } = new HashSet<ISensorActorRef>();

        protected HashSet<IActorRef> Subscribers { get; } = new HashSet<IActorRef>();

        protected ISimulatorModels Models;

        public RoomActor(RoomStatus initialStatus, ActorPath companySupervisorActorPath, ISimulatorModels models) : this()
        {
            Status = initialStatus;
            CompanySupervisorActorPath = companySupervisorActorPath;
            Models = models;
        }

        public RoomActor()
        {
            Receive<ParameterValueMessage>(
                msg => Status.Parameters[msg.ParamType].Value = msg.Value,
                msg => msg.ParamType != SensorType.Unknown
            );

            //Subscribtion handling
            Receive<SubscribeMessage>(message => OnSubscribeMessage());
            Receive<SubscriptionTriggerMessage>(message => SendSubscribtionNewsletter());
            Receive<UnsubscribeMessage>(message => OnUnsubscribeMesssage());

            Receive<ParameterValue>(message => this.Status.Parameters[message.ParameterType].Value = message.Value);

            Receive<RoomStatus.Request>(message =>
            {
                var status = GenerateRoomStatus();
                Sender.Tell(status, Self);
            });

            Receive<Requirements>(message =>
            {
                Controllers.Single(s => s.Type == SensorType.Temperature).Actor.Tell(message);
            });

            Receive<TemperatureJob>(message =>
            {
                foreach (var device in Status.Devices.Where(dev => dev is ITemperatureDevice).Cast<ITemperatureDevice>())
                    device.SetActiveMode(message.ModeType);
            });
        }

        protected virtual void OnUnsubscribeMesssage()
        {
            Subscribers.Remove(Sender);
        }

        protected virtual void SendSubscribtionNewsletter()
        {
            var status = GenerateRoomStatus();
            foreach (var subscriber in Subscribers)
                subscriber.Tell(status, Self);
        }

        protected virtual void OnSubscribeMessage()
        {
            Subscribers.Add(Sender);
            Sender.Tell(GenerateRoomStatus());
        }

        protected override void PreStart()
        {
            var selection = Context.System.ActorSelection(CompanySupervisorActorPath.ToString());
            selection.Tell(new RoomAvaliabilityMessage(Self));
            base.PreStart();
        }

        protected virtual IRoomStatusMessage GenerateRoomStatus()
        {
            //TODO To remove
            foreach (var sensor in Sensors)
                sensor.Actor.Tell(new ParameterValue.Request(sensor.Type));

            return Status.ToMessage();
        }
    }
}
