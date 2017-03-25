using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using System.Collections.Generic;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomActor : ReceiveActor
    {
        protected RoomStatus Status { get; } = new RoomStatus();

        protected ActorPath CompanySupervisorActorPath { get; }

        protected HashSet<ISensorActorRef> Sensors { get; } = new HashSet<ISensorActorRef>();

        protected HashSet<IActorRef> Subscribers { get; } = new HashSet<IActorRef>();

        public RoomActor(RoomStatus initialStatus, ActorPath companySupervisorActorPath) : this()
        {
            Status = initialStatus;
            CompanySupervisorActorPath = companySupervisorActorPath;
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

            Receive<RoomStatus.Request>(message =>
            {
                var status = GenerateRoomStatus();
                Sender.Tell(status, Self);
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
            return Status.ToMessage();
        }
    }
}
