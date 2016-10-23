using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Simulators;
using System.Collections.Generic;

namespace OfficeHVAC.Actors
{
    public class RoomSimulatorActor : ReceiveActor
    {
        private string RoomName { get; set; }

        private ITemperatureSimulator TemperatureSimulator { get; set; }

        private HashSet<IActorRef> Subscribers { get; set; } = new HashSet<IActorRef>();

        private ActorPath comparnySupervisorActorPath { get; set; }

        public RoomSimulatorActor(string roomName, ITemperatureSimulator temperatureSimulator, ActorPath companySupervisorActorPath)
        {
            this.RoomName = roomName;
            this.TemperatureSimulator = temperatureSimulator;
            this.comparnySupervisorActorPath = companySupervisorActorPath;

            this.Receive<SubscribeMessage>(message =>
            {
                this.Subscribers.Add(message.Subscriber);
                message.Subscriber.Tell(GenerateRoomStatus());
            });

            this.Receive<UnsubscribeMessage>(message =>
            {
                this.Subscribers.Remove(message.Subscriber);
            });

            this.Receive<RoomStatusRequest>(message =>
            {
                var status = GenerateRoomStatus();
                foreach (var subscriber in this.Subscribers)
                    subscriber.Tell(status, Self);
            });
        }

        public RoomStatusMessage GenerateRoomStatus()
        {
            return new RoomStatusMessage(RoomName, TemperatureSimulator.Temperature);
        }

        protected override void PreStart()
        {
            return;
            Context.System.ActorSelection(this.comparnySupervisorActorPath).Tell(new RoomAvaliabilityMessage(Self));
        }
    }
}
