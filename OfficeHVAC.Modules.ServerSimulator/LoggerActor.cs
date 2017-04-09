using System;
using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;
using NodaTime;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;

namespace OfficeHVAC.Modules.ServerSimulator
{
    public class LoggerActor : ReceiveActor
    {
        private List<IActorRef> rooms = new List<IActorRef>();

        public LoggerActor()
        {
            Debug.WriteLine($"Starting Logger on {Context.Self.Path}");

            Receive<RoomAvaliabilityMessage>(msg =>
            {
                Sender.Tell(new SubscribeMessage(Self));
                rooms.Add(Sender);

                Debug.WriteLine($"NEW ROOM [{Sender.Path}]");
                Context.Parent.Forward(msg);

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
                Debug.WriteLine($"UPDATE   [{ msg.Name }] T:{ msg.Parameters[SensorType.Temperature].Value }°C");
                Context.Parent.Forward(msg);
            });
        }

        protected override void PostStop()
        {
            base.PostStop();
            foreach (var room in rooms)
            {
                room.Tell(new UnsubscribeMessage(Self));
                Debug.WriteLine($"Unsubscribed from {room.Path}");
            }
            Debug.WriteLine("Shutting down");
        }
    }
}
