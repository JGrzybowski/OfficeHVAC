using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Remote;
using OfficeHVAC.Messages;

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

            Receive<RoomStatusMessage>(msg =>
            {
                Console.WriteLine($"UPDATE   [{msg.RoomName}] T:{msg.Temperature}°C");
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
