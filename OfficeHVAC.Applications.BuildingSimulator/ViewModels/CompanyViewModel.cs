using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class CompanyViewModel : BindableBase
    {
        private string name;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public ObservableCollection<RoomViewModel> Rooms { get; set; } = new ObservableCollection<RoomViewModel>();

        public CompanyViewModel(string name)
        {
            Name = name;

            for (int i = 0; i < 3; i++) Rooms.Add(new RoomViewModel());
        }
    }
}