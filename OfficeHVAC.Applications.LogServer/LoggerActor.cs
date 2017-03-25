using Akka.Actor;
using OfficeHVAC.Messages;
using System;
using System.Collections.Generic;
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
            });

            Receive<IRoomStatusMessage>(msg =>
            {
                Console.WriteLine($"UPDATE   [{ msg.Name }] T:{ msg.Parameters[SensorType.Temperature] }°C");
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
