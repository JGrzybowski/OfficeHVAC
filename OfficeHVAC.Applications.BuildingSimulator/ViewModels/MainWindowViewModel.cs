using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private bool isSimulatorRunning;
        public bool IsSimulatorRunning
        {
            get => isSimulatorRunning;
            set => OnSimulatorRunningChange(value);
        }

        public bool IsSimulatorStopped => !isSimulatorRunning;

        private void OnSimulatorRunningChange(bool isRunning)
        {
            isSimulatorRunning = isRunning;
            RaisePropertyChanged(nameof(IsSimulatorRunning));
            RaisePropertyChanged(nameof(IsSimulatorStopped));
        }

        public ObservableCollection<CompanyViewModel> Companies { get; set; } = new ObservableCollection<CompanyViewModel>();

        private object selectedItem;
        public object SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        public ICommand AddCompanyCommand { get; set; }
        public ICommand DeleteCompanyCommand { get; set; }

        public ICommand AddRoomCommand { get; set; }
        public ICommand RemoveRoomCommand { get; set; }

        public MainWindowViewModel()
        {
            AddCompanyCommand = new DelegateCommand(
                () => Companies.Add(new CompanyViewModel { Name = "NewCompany" }),
                () => IsSimulatorStopped
            ).ObservesCanExecute(() => IsSimulatorStopped);

            DeleteCompanyCommand = new DelegateCommand(
                    () => Companies.Remove(SelectedItem as CompanyViewModel),
                    () => IsSimulatorStopped && SelectedItem is CompanyViewModel
                )
                .ObservesProperty(() => SelectedItem)
                .ObservesProperty(() => IsSimulatorStopped);

            AddRoomCommand = new DelegateCommand(
                    () => (SelectedItem as CompanyViewModel).AddRoom(new RoomViewModel { Name = "New Room" }),
                    () => IsSimulatorStopped && SelectedItem is CompanyViewModel)
                .ObservesProperty(() => SelectedItem)
                .ObservesProperty(() => IsSimulatorStopped);

            RemoveRoomCommand = new DelegateCommand
                (
                    () =>
                    {
                        var roomId = (SelectedItem as RoomViewModel).Id;
                        var company = Companies.Single(c => c.SubItems.Any(r => r.Id == roomId));
                        company.RemoveRoom(roomId);
                    },
                    () => IsSimulatorStopped && SelectedItem is RoomViewModel
                )
                .ObservesProperty(() => SelectedItem)
                .ObservesProperty(() => IsSimulatorStopped);

            //SeedData();
        }

        private void SeedData()
        {
            Companies.Add(new CompanyViewModel()
            {
                Name = "CompanyA",
                SubItems =
                {
                    new RoomViewModel() {Name = "Room 101"},
                    new RoomViewModel() {Name = "Room 102"}
                }
            });

            Companies.Add(new CompanyViewModel()
            {
                Name = "CompanyB",
                SubItems =
                {
                    new RoomViewModel() {Name = "Room 1053"},
                    new RoomViewModel() {Name = "Room 1026"}
                }
            });
            Companies.Add(new CompanyViewModel()
            {
                Name = "CompanyC",
                SubItems =
                {
                    new RoomViewModel() {Name = "Room 57"},
                    new RoomViewModel() {Name = "Room 1021"}
                }
            });
        }
    }
}
