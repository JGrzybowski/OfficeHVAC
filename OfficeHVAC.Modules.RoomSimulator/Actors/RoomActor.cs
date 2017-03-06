using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public abstract class RoomActor : ReceiveActor
    {
        protected string RoomName { get; }

        protected HashSet<ISensorActor> Sensors { get; } = new HashSet<ISensorActor>();

        protected HashSet<IActorRef> Subscribers { get; } = new HashSet<IActorRef>();

        public RoomActor(string roomName)
        {
            RoomName = roomName;

            //Subscribtion handling
            this.ReceiveAsync<SubscribeMessage>(async message =>
            {
                this.Subscribers.Add(Sender);
                Sender.Tell(await GenerateRoomStatus());
            });

            this.Receive<UnsubscribeMessage>(message =>
            {
                this.Subscribers.Remove(Sender);
            });

            this.ReceiveAsync<RoomStatusRequest>(async message =>
            {
                var status = await GenerateRoomStatus();
                foreach (var subscriber in this.Subscribers)
                    subscriber.Tell(status, Self);
            });
        }

        protected virtual async Task<RoomStatusMessage> GenerateRoomStatus()
        {
            var tempSensors = Sensors.Where(s => s.Type == SensorTypes.Temperature);
            var temperatures = await Task.WhenAll(tempSensors.Select(s => s.Actor.Ask<TemperatureValueMessage>(new TemperatureValueRequest(), TimeSpan.FromSeconds(5))));
            var temperature = temperatures.Average(t => t.Temperature);

            return new RoomStatusMessage(RoomName, temperature);
        }
    }
}
