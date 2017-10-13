using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Akka.Actor;
using OfficeHVAC.Applications.BuildingSimulator.Actors;
using Prism.Commands;
using Prism.Mvvm;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class BuildingViewModel : BindableBase
    {
        public ObservableCollection<CompanyViewModel> Companies { get; set; } = new ObservableCollection<CompanyViewModel>();
        
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

        public ActorSystem ActorSystem { get; set; }
        
        public BuildingViewModel(ActorSystem actorSystem)
        {
            ActorSystem = actorSystem;
            
            InitializeAddCompanyCommand();
            InitializeDeleteCompanyCommand();

            InitializeAddRoomCommand();
            InitializeRemoveRoomCommand();

            InitializeAddDeviceCommand();
            InitializeRemoveDeviceCommand();
        }
        
        public ICommand AddCompanyCommand { get; set; }
        public CompanyViewModel AddCompany()
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
            AddCompanyCommand = new DelegateCommand(() => AddCompany());
        }

        public ICommand DeleteCompanyCommand { get; set; }
        public void RemoveCompany(CompanyViewModel company) => Companies.Remove(company);
        private void InitializeDeleteCompanyCommand()
        {
            DeleteCompanyCommand = new DelegateCommand(
                    () => RemoveCompany(SelectedCompany),
                    () => CompanyIsSelected
                )
                .ObservesProperty(() => SelectedItem);
        }

        public ICommand AddRoomCommand { get; set; }
        private void InitializeAddRoomCommand()
        {
            AddRoomCommand = new DelegateCommand(
                    async () => await SelectedCompany.AddRoom(),
                          () => CompanyIsSelected)
                .ObservesProperty(() => SelectedItem);
        }

        public ICommand RemoveRoomCommand { get; set; }
        public void RemoveRoom(RoomViewModel room)
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
                    () => RoomIsSelected
                )
                .ObservesProperty(() => SelectedItem);
        }

        public ICommand AddDeviceCommand { get; set; }
        public DeviceViewModel AddDevice(RoomViewModel room)
        {
            var device = new DeviceViewModel {Name = "New Device"};
            room.AddDevice(device);
            return device;
        }
        private void InitializeAddDeviceCommand()
        {
            AddDeviceCommand = new DelegateCommand(
                    () => AddDevice(SelectedRoom),
                    () => RoomIsSelected
                )
                .ObservesProperty(() => SelectedItem);
        }

        public ICommand RemoveDeviceCommand { get; set; }
        public void RemoveDevice(DeviceViewModel deviceViewModel)
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
                    () => DeviceIsSelected
                )
                .ObservesProperty(() => SelectedItem);
        }
    }
}