using Akka.Actor;
using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using Prism.Mvvm;
using System;
using RoomSimulatorActor = OfficeHVAC.Modules.RoomSimulator.Actors.RoomSimulatorActor;

namespace OfficeHVAC.Modules.RoomSimulator.Factories
{
    public class RoomSimulatorActorPropsFactory : BindableBase, IRoomSimulatorActorPropsFactory
    {
        public IActorPathBuilder CompanyPathBuilder { get; }

        public ITemperatureSimulatorFactory TemperatureSimulatorFactory { get; }

        public RoomSimulatorActorPropsFactory(IActorPathBuilder companyPathBuilder, ITemperatureSimulatorFactory temperatureSimulatorFactory)
        {
            CompanyPathBuilder = companyPathBuilder;
            TemperatureSimulatorFactory = temperatureSimulatorFactory;
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
                Parameters = new ParameterValuesCollection() { new ParameterValue(SensorType.Temperature, 20) }
            };

            return Akka.Actor.Props.Create(() => new RoomSimulatorActor(
                initialRoomStatus,
                CompanyPathBuilder.ActorPath(),
                TemperatureSimulatorFactory
            ));
        }
    }
}
