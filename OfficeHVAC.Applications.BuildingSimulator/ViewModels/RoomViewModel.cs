using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.RoomSimulator.Messages;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;
using OfficeHVAC.Applications.BuildingSimulator.Actors;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class RoomViewModel : BindableBase, ITreeElement
    {
        private readonly ActorSystem actorSystem;
        
        public string Id { get; } = Guid.NewGuid().ToString();

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private RoomStatus status = new RoomStatus();
        public RoomStatus Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }
        
        public ObservableCollection<ITreeElement> Devices { get; } = new ObservableCollection<ITreeElement>();
        public ObservableCollection<ITreeElement> Sensors { get; } = new ObservableCollection<ITreeElement>();
        public ObservableCollection<ITreeElement> SubItems { get; }

        public Instant Timestamp => Status.TimeStamp;

        public double Temperature
        {
            get => (double)(Status.Parameters.TryGet(SensorType.Temperature).Value);
            set
            {
                Status.Parameters.TryGet(SensorType.Temperature).Value = value;
                Actor.Tell(new SetTemperature(value));
            }
        }

        public double Volume
        {
            get => Status.Volume;
            set
            {
                Status.Volume = value;
                Actor.Tell(new SetRoomVolume(value));
            }
        }
        
        public IActorRef Actor { get; set; }

        public RoomViewModel(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
            SubItems = new ObservableCollection<ITreeElement>()
            {
                new TreeElement(Sensors){Name = "Sensors"},
                new TreeElement(Devices){Name = "Devices"}
            };
        }
        
        public void AddActuator(SensorViewModel device) => Devices.Add(device);
        public void RemoveDevice(string id) => Devices.Remove(Devices.Single(dev => dev.Id == id));

        public void AddSensor(SensorViewModel sensor) => Sensors.Add(sensor);
        public void RemoveSensor(string id) => Sensors.Remove(Sensors.Single(snr => snr.Id == id));

        public async Task AddTemperatureSensor(TemperatureSensorViewModel sensor, string timeSimulatorActorPath, string tempSimulatorModelActorPath)
        {
            var tempSensorActor = await Actor.Ask<IActorRef>(new AddTemperatureSensorMessage(timeSimulatorActorPath, tempSimulatorModelActorPath, sensor.Id));
            var bridgeActor = actorSystem.ActorOf(TemperatureSimulatorBridgeActor.Props(sensor, tempSensorActor));
            
            sensor.Actor = bridgeActor;
            AddSensor(sensor);
        }

        public async Task AddTemperatureActuator(TemperatureActuatorViewModel viewModel, string timeSimulatorActorPath, string tempSimulatorModelActorPath)
        {
            var temperatureActuatorActor = await Actor.Ask<IActorRef>(new AddTemperatureActuatorMessage(viewModel.Id));
            var bridgeActor = actorSystem.ActorOf(TemperatureActuatorBridgeActor.Props(viewModel, temperatureActuatorActor));

            viewModel.Actor = bridgeActor;
            AddActuator(viewModel);
        }
    }
}