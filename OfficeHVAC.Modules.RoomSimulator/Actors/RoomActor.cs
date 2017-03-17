using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomActor : ReceiveActor
    {
        protected RoomInfo RoomInfo { get; } = new RoomInfo();

        protected ParameterValuesCollection Parameters { get; } = new ParameterValuesCollection();

        protected ActorPath CompanySupervisorActorPath { get; }

        protected HashSet<ISensorActorRef> Sensors { get; } = new HashSet<ISensorActorRef>();

        protected HashSet<IActorRef> Subscribers { get; } = new HashSet<IActorRef>();

        public RoomActor(RoomInfo roomInfo, ActorPath companySupervisorActorPath, ParameterValuesCollection parameters) : this()
        {
            RoomInfo = roomInfo;
            CompanySupervisorActorPath = companySupervisorActorPath;
            Parameters = parameters;
        }

        public RoomActor()
        {
            Receive<ParameterValueMessage>(
                msg => Parameters[msg.ParamType].Value = msg.Value,
                msg => msg.ParamType != SensorType.Unknown
            );

            //Subscribtion handling
            Receive<SubscribeMessage>(message =>
            {
                Subscribers.Add(Sender);
                Sender.Tell(GenerateRoomStatus());
            });

            Receive<SubscriptionTriggerMessage>(message =>
            {
                var status = GenerateRoomStatus();
                foreach (var subscriber in Subscribers)
                    subscriber.Tell(status, Self);
            });

            Receive<UnsubscribeMessage>(message =>
            {
                Subscribers.Remove(Sender);
            });

            Receive<RoomStatus.Request>(message =>
            {
                var status = GenerateRoomStatus();
                Sender.Tell(status, Self);
            });
        }

        protected override void PreStart()
        {
            var selection = Context.System.ActorSelection(CompanySupervisorActorPath.ToString());
            selection.Tell(new RoomAvaliabilityMessage(Self));
            base.PreStart();
        }

        protected virtual IRoomStatusMessage GenerateRoomStatus()
        {
            var status = new RoomStatus()
            {
                RoomInfo = RoomInfo,
                Parameters = Parameters,
                Sensors = new List<ISensorActorRef>()
            };
            return status.ToMessage();
        }

    }
}
