using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.RoomSimulator.Actors;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;

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

            Receive<ChangeNameMessage>(msg =>
            {
                Name = msg.NewValue;
                Thread.Sleep(5000);
                Sender.Tell(new UpdateCompanyMessage($"{Name} from Actor"));
            });
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
            
            var props = Props.Create(() =>
                new RoomSimulatorActor(initialStatus, Self.Path, new TemperatureSimulatorFactory(null, null), null));
            var actor = Context.ActorOf(props);
            
            RoomsActors.Add(msg.Id, actor);
            return actor;
        }
    }
}