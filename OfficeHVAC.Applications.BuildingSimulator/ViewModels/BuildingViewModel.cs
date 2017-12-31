using Akka.Actor;
using OfficeHVAC.Applications.BuildingSimulator.Actors;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class BuildingViewModel : BindableBase
    {
        public ObservableCollection<CompanyViewModel> Companies { get; set; } = new ObservableCollection<CompanyViewModel>();
        
        private object selectedItem;
        public object SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        public CompanyViewModel SelectedCompany => SelectedItem as CompanyViewModel;
        public RoomViewModel SelectedRoom       => SelectedItem as RoomViewModel;
        public DeviceViewModel SelectedDevice   => SelectedItem as DeviceViewModel;
        public SensorViewModel SelectedSensor   => SelectedItem as SensorViewModel;

        public bool CompanyIsSelected   => SelectedCompany  != null;
        public bool RoomIsSelected      => SelectedRoom     != null;
        public bool DeviceIsSelected    => SelectedDevice   != null;
        public bool SensorIsSelected    => SelectedSensor   != null;

        public ActorSystem ActorSystem { get; set; }
        
        public BuildingViewModel(ActorSystem actorSystem)
        {
            ActorSystem = actorSystem;
            
            InitializeAddCompanyCommand();
            InitializeDeleteCompanyCommand();

            InitializeAddRoomCommand();
            InitializeRemoveRoomCommand();

            InitializeAddActuatorCommand();
            //InitializeAddDeviceCommand();
            InitializeRemoveDeviceCommand();
            
            InitializeAddTemperatureSensorCommand();
            InitializeRemoveSensorCommand();
        }
        
        //Company Commands
        public ICommand AddCompanyCommand { get; set; }
        public CompanyViewModel AddCompany()
        {
            var companyActor = ActorSystem.ActorOf<CompanyActor>();
            
            var companyVm = new CompanyViewModel(ActorSystem)
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

        //Room Commands
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

        //Actuator Commands
        public ICommand AddTemperatureActuatorCommand { get; set; }
        public async Task<TemperatureControllerViewModel> AddTemperatureActuator(RoomViewModel room)
        {
            var actuator = new TemperatureControllerViewModel {Name = "Temperature Controller"};
            await room.AddTemperatureController(actuator, SystemInfo.TimeSimulatorActorPath, SystemInfo.TempSimulatorModelActorPath);
            return actuator;
        }
        private void InitializeAddActuatorCommand()
        {
            AddTemperatureActuatorCommand = new DelegateCommand(
                    async () => await AddTemperatureActuator(SelectedRoom),
                          () => RoomIsSelected
                )
                .ObservesProperty(() => SelectedItem);
        }
        
        //Device Commands
        //TODO add model for adding device
        //public ICommand AddDeviceCommand { get; set; }
        //private void AddDevice(RoomViewModel room, TemperatureDeviceDefinition temperatureDeviceDefinition) => 
        //    room.AddTemperatureDevice(temperatureDeviceDefinition);
        //private void InitializeAddDeviceCommand()
        //{
        //    AddDeviceCommand = new DelegateCommand(
        //            () => AddDevice(SelectedRoom, temperatureDeviceDefinition),
        //            () => RoomIsSelected
        //        )
        //        .ObservesProperty(() => SelectedItem);
        //}

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
        
        //Sensor Commands
        public ICommand AddTemperatureSensorCommand { get; set; }
        public async Task<SensorViewModel> AddTemperatureSensor(RoomViewModel room)
        {
            var sensor = new TemperatureSensorViewModel() {Name = "Temparature Sensor"};
            await room.AddTemperatureSensor(sensor, SystemInfo.TimeSimulatorActorPath, SystemInfo.TempSimulatorModelActorPath);
            return sensor;
        }
        private void InitializeAddTemperatureSensorCommand()
        {
            AddTemperatureSensorCommand = new DelegateCommand(
                    async () => await AddTemperatureSensor(SelectedRoom),
                          () => RoomIsSelected
                )
                .ObservesProperty(() => SelectedItem);
        }

        public ICommand RemoveSensorCommand { get; set; }
        public void RemoveSensor(SensorViewModel sensorViewModel)
        {
            var sensorId = SelectedSensor.Id;
            var room = Companies.SelectMany(c => c.SubItems)
                    .Single(r => r.SubItems.Any(snr => snr.Id == sensorId))
                as RoomViewModel;
            room?.RemoveDevice(sensorId);
        }
        private void InitializeRemoveSensorCommand()
        {
            RemoveSensorCommand = new DelegateCommand
                (
                    () => RemoveSensor(SelectedSensor),
                    () => SensorIsSelected
                )
                .ObservesProperty(() => SelectedItem);
        }
    }
}