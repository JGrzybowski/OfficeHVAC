using Akka.Actor;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.RoomSimulator.Actors;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using System.Collections.Generic;

namespace OfficeHVAC.Applications.BuildingSimulator.Actors
{
    public class CompanyActor : ReceiveActor
    {
        public string Name { get; set; }
        
        public Dictionary<string, IActorRef> RoomsActors { get; set; } = new Dictionary<string, IActorRef>();
        
        public CompanyActor()
        {
            Receive<CreateRoomMessage>(msg => Sender.Tell(CreateNewRoom(msg)));
            Receive<RemoveRoomMessage>(msg => RemoveRoom(msg.Id));

            Receive<ChangeNameMessage>(msg => Name = msg.NewValue);

            Receive<TimeChangedMessage>(msg => SendToAllRooms(msg));

            var selection = Context.System.ActorSelection($"user/{SystemInfo.TimeSimulatorActorName}");
            selection.Tell(new SubscriptionMessage());
        }

        private void RemoveRoom(string id)
        {
            RoomsActors[id].Tell(PoisonPill.Instance);
            RoomsActors.Remove(id);
        }

        private IActorRef CreateNewRoom(CreateRoomMessage msg)
        {
            //TODO how to distribute TemperatureModels??
            RoomStatus initialStatus = new RoomStatus()
            {
                Id = msg.Id,
                Name = msg.Name
            };
            
            var props = RoomSimulatorActor.Props(initialStatus, Self.Path.ToStringWithoutAddress(), SystemInfo.TimeSimulatorActorPath, SystemInfo.TempSimulatorModelActorPath);
            var actor = Context.ActorOf(props);
            
            RoomsActors.Add(msg.Id, actor);
            return actor;
        }

        private void SendToAllRooms(object msg)
        {
            foreach (var room in RoomsActors)
                room.Value.Tell(msg);
        }
    }
}