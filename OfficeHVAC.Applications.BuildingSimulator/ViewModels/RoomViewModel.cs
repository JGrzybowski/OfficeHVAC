using OfficeHVAC.Models;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Dsl;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class RoomViewModel : BindableBase, ITreeElement
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private RoomStatus status;
        public RoomStatus Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }
        
        public ObservableCollection<ITreeElement> Devices { get; } = new ObservableCollection<ITreeElement>();
        public ObservableCollection<ITreeElement> Sensors { get; } = new ObservableCollection<ITreeElement>();
        public ObservableCollection<ITreeElement> SubItems { get; }

        public double Temperature { get; set; } = 20.0;
        public double Volume { get; set; } = 300;
        public IActorRef Actor { get; set; }

        public RoomViewModel()
        {
            SubItems = new ObservableCollection<ITreeElement>()
            {
                new TreeElement(Sensors){Name = "Sensors"},
                new TreeElement(Devices){Name = "Devices"}
            };
        }
        
        public void AddDevice(DeviceViewModel device) => Devices.Add(device);
        public void RemoveDevice(string id) => Devices.Remove(Devices.Single(dev => dev.Id == id));

        public void AddSensor(SensorViewModel sensor) => Sensors.Add(sensor);
        public void RemoveSensor(string id) => Sensors.Remove(Sensors.Single(snr => snr.Id == id));

        public async Task AddTemperatureSensor(TemperatureSensorViewModel sensor, string timeSimulatorActorPath, string tempSimulatorModelActorPath)
        {
            var tempSensorActor = await Actor.Ask<IActorRef>(new AddTemperatureSensorMessage(timeSimulatorActorPath, tempSimulatorModelActorPath, sensor.Id));
            sensor.Actor = tempSensorActor;
            AddSensor(sensor);
        }
    }
}