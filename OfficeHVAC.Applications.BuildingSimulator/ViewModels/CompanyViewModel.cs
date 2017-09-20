using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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

        public ObservableCollection<ITreeElement> SubItems { get; } = new ObservableCollection<ITreeElement>();

        public void AddRoom(RoomViewModel room)
        {
            SubItems.Add(room);
        }

        public void RemoveRoom(string id)
        {
            SubItems.Remove(SubItems.Single(room => room.Id == id));
        }
    }
}