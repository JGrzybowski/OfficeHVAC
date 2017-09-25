using Akka.Actor;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using OfficeHVAC.Applications.BuildingSimulator.Actors;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private bool isSimulatorRunning;
        public bool IsSimulatorRunning
        {
            get => isSimulatorRunning;
            set
            {
                isSimulatorRunning = value;
                RaisePropertyChanged(nameof(IsSimulatorRunning));
                RaisePropertyChanged(nameof(IsSimulatorStopped));
            }
        }

        public bool IsSimulatorStopped => !isSimulatorRunning;  

        public ObservableCollection<CompanyViewModel> Companies { get; set; } = new ObservableCollection<CompanyViewModel>();

        public ActorSystem ActorSystem { get; set; }

        private object selectedItem;
        public object SelectedItem
        {
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);
                //RaisePropertyChanged(nameof(SelectedCompany));
                //RaisePropertyChanged(nameof(SelectedRoom));
                //RaisePropertyChanged(nameof(SelectedDevice));
            }
        }

        public CompanyViewModel SelectedCompany => SelectedItem as CompanyViewModel;
        public RoomViewModel SelectedRoom       => SelectedItem as RoomViewModel;
        public DeviceViewModel SelectedDevice   => SelectedItem as DeviceViewModel;

        public bool CompanyIsSelected   => SelectedCompany  != null;
        public bool RoomIsSelected      => SelectedRoom     != null;
        public bool DeviceIsSelected    => SelectedDevice   != null;

        public ICommand AddCompanyCommand { get; set; }
        private CompanyViewModel AddCompany()
        {
            var companyActor = ActorSystem.ActorOf<CompanyActor>();
            
            var companyVm = new CompanyViewModel
            {
                Name = "NewCompany",
            };

            var bridgeProps = Props.Create(() => new CompanyBridgeActor(companyActor, companyVm));
            var bridgeActor = ActorSystem.ActorOf(bridgeProps);
            companyVm.Actor = bridgeActor;
            
            Companies.Add(companyVm);
            return companyVm;
        }
        private void InitializeAddCompanyCommand()
        {
            AddCompanyCommand = new DelegateCommand(
                    () => AddCompany(),
                    () => IsSimulatorStopped
                )
                .ObservesCanExecute(() => IsSimulatorStopped);
        }

        public ICommand DeleteCompanyCommand { get; set; }
        private void RemoveCompany(CompanyViewModel company) => Companies.Remove(company);
        private void InitializeDeleteCompanyCommand()
        {
            DeleteCompanyCommand = new DelegateCommand(
                    () => RemoveCompany(SelectedCompany),
                    () => IsSimulatorStopped && CompanyIsSelected
                )
                .ObservesProperty(() => SelectedItem)
                .ObservesProperty(() => IsSimulatorStopped);
        }

        public ICommand AddRoomCommand { get; set; }
        private void InitializeAddRoomCommand()
        {
            AddRoomCommand = new DelegateCommand(
                    async () => await SelectedCompany.AddRoom(),
                          () => IsSimulatorStopped && CompanyIsSelected)
                .ObservesProperty(() => SelectedItem)
                .ObservesProperty(() => IsSimulatorStopped);
        }

        public ICommand RemoveRoomCommand { get; set; }
        private void RemoveRoom(RoomViewModel room)
        {
            var roomId = room.Id;
            var company = Companies.Single(c => c.SubItems.Any(r => r.Id == roomId));
            company.RemoveRoom(roomId);
        }
        private void InitializeRemoveRoomCommand()
        {
            RemoveRoomCommand = new DelegateCommand
                (
                    () => RemoveRoom(SelectedRoom),
                    () => IsSimulatorStopped && RoomIsSelected
                )
                .ObservesProperty(() => SelectedItem)
                .ObservesProperty(() => IsSimulatorStopped);
        }

        public ICommand AddDeviceCommand { get; set; }
        private DeviceViewModel AddDevice(RoomViewModel room)
        {
            var device = new DeviceViewModel {Name = "New Device"};
            room.AddDevice(device);
            return device;
        }
        private void InitializeAddDeviceCommand()
        {
            AddDeviceCommand = new DelegateCommand(
                    () => AddDevice(SelectedRoom),
                    () => IsSimulatorStopped && RoomIsSelected
                )
                .ObservesProperty(() => SelectedItem)
                .ObservesProperty(() => IsSimulatorStopped);
        }

        public ICommand RemoveDeviceCommand { get; set; }
        private void RemoveDevice(DeviceViewModel deviceViewModel)
        {
            var deviceId = SelectedDevice.Id;
            var room = Companies.SelectMany(c => c.SubItems)
                                .Single(r => r.SubItems.Any(dev => dev.Id == deviceId))
                                as RoomViewModel;
            room?.RemoveDevice(deviceId);
        }
        private void InitializeRemoveDeviceCommand()
        {
            RemoveDeviceCommand = new DelegateCommand
                (
                    () => RemoveDevice(SelectedDevice),
                    () => IsSimulatorStopped && DeviceIsSelected
                )
                .ObservesProperty(() => SelectedItem)
                .ObservesProperty(() => IsSimulatorStopped);
        }

        public MainWindowViewModel()
        {
            ActorSystem = ActorSystem.Create("OfficeHVAC");
            
            InitializeAddCompanyCommand();
            InitializeDeleteCompanyCommand();

            InitializeAddRoomCommand();
            InitializeRemoveRoomCommand();

            InitializeAddDeviceCommand();
            InitializeRemoveDeviceCommand();

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
