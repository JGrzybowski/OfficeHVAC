using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Simulators;
using System.Collections.Generic;

namespace OfficeHVAC.Actors
{
    public class RoomSimulatorActor : ReceiveActor
    {
        public string RoomName { get; set; }

        public ITemperatureSimulator TemperatureSimulator { get; set; }

        public HashSet<IActorRef> Subscribers { get; set; } = new HashSet<IActorRef>();

        public RoomSimulatorActor(string roomName, ITemperatureSimulator temperatureSimulator)
        {
            this.RoomName = roomName;
            this.TemperatureSimulator = temperatureSimulator;

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
    }
}
