using Akka.Actor;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;

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

        public ActorSystem ActorSystem { get; set; }

        public BuildingViewModel Building { get; set; }
        public ITimeControlViewModel Time { get; set; }
        public TemperatureSimulatorParamsViewModel TemperatureParams { get; set; }
        
        public MainWindowViewModel(ActorSystem actorSystem, BuildingViewModel building, 
            ITimeControlViewModel time, TemperatureSimulatorParamsViewModel temperatureParams)
        {
            ActorSystem = actorSystem;
            Building = building;
            Time = time;
            TemperatureParams = temperatureParams;
        }

        public async Task SeedData()
        {
            var company = Building.AddCompany();
            company.Name = "CompanyA";
            
            var room = await company.AddRoom();
            room.Volume = 300;
            room.Temperature = 25;
            room.Name = "Room 101";  
            
            var sensor = await Building.AddTemperatureSensor(room);
            var controller = await Building.AddTemperatureActuator(room);
            
            Thread.Sleep(1000);
            
            room.AddTemperatureDevice(new TemperatureDeviceDefinition
                (
                    id : "Test dev",
                    maxPower : 5000,
                    modes : new List<ITemperatureMode>()
                    {
                        new TemperatureMode()
                        {
                            Name = "Off",
                            Type = TemperatureModeType.Off,
                            TemperatureRange = new Range<double>(-100, 100)
                        },
                        new TemperatureMode()
                        {
                            Name = "Stabilization",
                            Type = TemperatureModeType.Stabilization,
                            PowerEfficiency = 0.99,
                            PowerConsumption = 0.1,
                            TemperatureRange = new Range<double>(-3, 3)
                        },
                        new TemperatureMode()
                        {
                            Name = "Eco",
                            Type = TemperatureModeType.Eco,
                            PowerEfficiency = 0.93,
                            PowerConsumption = 0.3,
                            TemperatureRange = new Range<double>(-100, 100)
                        }
                    }
                )
            );

            var meetingA = new Requirement<double>(Time.Time + Duration.FromHours(2), Time.Time + Duration.FromHours(3), 21.0);
            var meetingB = new Requirement<double>(meetingA.EndTime + Duration.FromMinutes(15), meetingA.EndTime + Duration.FromHours(2), 24.0);
            var meetingC = new Requirement<double>(meetingB.EndTime + Duration.FromMinutes(60), meetingB.EndTime + Duration.FromHours(2), 18.0);
            //controller.Tell(new {meetingA, meetingB, meetingC});

            //room = await company.AddRoom();
            //room.Name = "Room 102";

            //sensor = await Building.AddTemperatureSensor(room);

//            company = Building.AddCompany();
//            company.Name = "CompanyB";
//            
//            room = await company.AddRoom();
//            room.Name = "Room 1053";
//            room = await company.AddRoom();
//            room.Name = "Room 1026";
//            
//            company = Building.AddCompany();
//            company.Name = "CompanyC";
//            
//            room = await company.AddRoom();
//            room.Name = "Room 57";
//            room = await company.AddRoom();
//            room.Name = "Room 1021";
        }
    }
}
