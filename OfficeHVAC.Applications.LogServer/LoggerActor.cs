using Akka.Actor;
using OfficeHVAC.Messages;
using System;
using System.Collections.Generic;
using NodaTime;
using OfficeHVAC.Models;

namespace OfficeHVAC.Applications.LogServer
{
    public class LoggerActor : ReceiveActor
    {
        private List<IActorRef> rooms = new List<IActorRef>();

        public LoggerActor()
        {
            Receive<RoomAvaliabilityMessage>(msg =>
            {
                Sender.Tell(new SubscribeMessage(Self));
                rooms.Add(Sender);
                Console.WriteLine($"NEW ROOM [{Sender.Path}]");
                Sender.Tell(
                    new Requirements(
                        Instant.FromDateTimeUtc(DateTime.UtcNow) + Duration.FromHours(12),
                        new ParameterValuesCollection
                        {
                            new ParameterValue(SensorType.Temperature, 18)
                        }));
            });

            Receive<IRoomStatusMessage>(msg =>
            {
                Console.WriteLine($"UPDATE   [{ msg.Name }] T:{ msg.Parameters[SensorType.Temperature].Value }°C");
            });
        }

        protected override void PostStop()
        {
            base.PostStop();
            foreach (var room in rooms)
            {
                room.Tell(new UnsubscribeMessage(Self));
                Console.WriteLine($"Unsubscribed from {room.Path}");
            }
            Console.WriteLine("Shutting down");
        }
    }
}
