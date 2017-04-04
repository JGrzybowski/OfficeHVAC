using Akka.Actor;
using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using OfficeHVAC.Models.Devices;
using RoomSimulatorActor = OfficeHVAC.Modules.RoomSimulator.Actors.RoomSimulatorActor;

namespace OfficeHVAC.Modules.RoomSimulator.Factories
{
    public class RoomSimulatorActorPropsFactory : BindableBase, IRoomSimulatorActorPropsFactory
    {
        public IActorPathBuilder CompanyPathBuilder { get; }

        public ITemperatureSimulatorFactory TemperatureSimulatorFactory { get; }

        public ISimulatorModels Models { get; set; }

        public RoomSimulatorActorPropsFactory(IActorPathBuilder companyPathBuilder,
                                              ITemperatureSimulatorFactory temperatureSimulatorFactory,
                                              ISimulatorModels models)
        {
            CompanyPathBuilder = companyPathBuilder;
            TemperatureSimulatorFactory = temperatureSimulatorFactory;
            Models = models;
        }

        public float Temperature { get; set; }

        private string _roomName = "Room";
        public string RoomName
        {
            get { return _roomName; }
            set { SetProperty(ref _roomName, value); }
        }

        public Props Props()
        {
            if (string.IsNullOrWhiteSpace(RoomName))
                throw new ArgumentException("Room name cannot be empty.", nameof(RoomName));

            var initialRoomStatus = new RoomStatus()
            {
                Name = RoomName,
                Parameters = new ParameterValuesCollection() { new ParameterValue(SensorType.Temperature, 20) },
                Volume = 100,
                Devices = new HashSet<IDevice>()
                {
                    new TemperatureDevice()
                    {
                        Id = Guid.NewGuid().ToString(),
                        MaxPower = 2000,
                        Modes = new ModesCollection
                        {
                            new TemperatureMode() {Name = "Off", Type = TemperatureModeType.Off, TemperatureRange = new Range<double>(-100,100) },
                            new TemperatureMode() {Name = "Eco", Type = TemperatureModeType.Eco, PowerEfficiency = 0.95, PowerConsumption = 0.3, TemperatureRange = new Range<double>(-100,100)},
                            new TemperatureMode() {Name = "Turbo", Type = TemperatureModeType.Turbo, PowerEfficiency = 0.50, PowerConsumption = 1.0, TemperatureRange = new Range<double>(-100,100)}
                        }
                    }
                }
            };

            return Akka.Actor.Props.Create(() => new RoomSimulatorActor(
                initialRoomStatus,
                CompanyPathBuilder.ActorPath(),
                TemperatureSimulatorFactory,
                Models
            ));
        }
    }
}
