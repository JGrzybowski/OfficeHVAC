using Akka.Actor;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OfficeHVAC.Applications.BuildingSimulator.Actors;
using OfficeHVAC.Models;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class CompanyViewModel : BindableBase, ITreeElement
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public IActorRef Actor { get; set; }
    
        public ObservableCollection<ITreeElement> SubItems { get; } = new ObservableCollection<ITreeElement>();

        public async Task<RoomViewModel> AddRoom()
        {
            var room = new RoomViewModel {Name = "New Room"};
            
            var roomActorRef = Actor.Ask<IActorRef>(new CreateRoomMessage()
            {
                Id = room.Id,
                Name = room.Name
            });

            room.Actor = await roomActorRef;
            
            SubItems.Add(room);
            return room;
        }

        public void RemoveRoom(string id)
        {
            var roomVm = SubItems.Single(room => room.Id == id);
            this.Actor.Tell(new RemoveRoomMessage() {Id = roomVm.Id});
            SubItems.Remove(roomVm);
        }
    }
}