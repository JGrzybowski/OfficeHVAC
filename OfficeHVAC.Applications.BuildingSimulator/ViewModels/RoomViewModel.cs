using OfficeHVAC.Models;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class RoomViewModel : BindableBase, ITreeElement
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        private string title;
        public string Name
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private RoomStatus status;
        public RoomStatus Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public ObservableCollection<ITreeElement> SubItems { get; } = new ObservableCollection<ITreeElement>();

        public double Temperature { get; set; } = 20.0;
        public double Volume { get; set; } = 300;
        public IActorRef Actor { get; set; }

        public void AddDevice(DeviceViewModel device) => SubItems.Add(device);

        public void RemoveDevice(string id) => SubItems.Remove(SubItems.Single(dev => dev.Id == id));
    }
}