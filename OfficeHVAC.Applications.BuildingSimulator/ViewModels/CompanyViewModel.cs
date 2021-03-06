﻿using Akka.Actor;
using OfficeHVAC.Applications.BuildingSimulator.Actors;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class CompanyViewModel : BindableBase, ITreeElement
    {
        private readonly ActorSystem actorSystem;
        public string Id { get; } = Guid.NewGuid().ToString();
        public CompanyViewModel(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        private string name;    
        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
                Actor?.Tell(new ChangeNameMessage(value));
            }
        }

        public IActorRef Actor { get; set; }
    
        public ObservableCollection<ITreeElement> SubItems { get; } = new ObservableCollection<ITreeElement>();

        public async Task<RoomViewModel> AddRoom()
        {
            var room = new RoomViewModel(actorSystem) { Name = "New Room" };
            
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
            Actor.Tell(new RemoveRoomMessage() {Id = roomVm.Id});
            SubItems.Remove(roomVm);
        }

        public void PushName(string value) => SetProperty(ref name, value, nameof(Name));
    }
}