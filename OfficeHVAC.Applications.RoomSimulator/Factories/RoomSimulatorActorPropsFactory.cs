using Akka.Actor;
using OfficeHVAC.Actors;
using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Factories.Simulators.Temperature;
using Prism.Mvvm;
using System;

namespace OfficeHVAC.Applications.RoomSimulator.Factories
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

        private string _roomName;
        public string RoomName
        {
            get { return _roomName; }
            set { SetProperty(ref _roomName, value); }
        }


        public Props Props()
        {
            if (string.IsNullOrWhiteSpace(RoomName))
                throw new ArgumentException("Room name cannot be empty.", nameof(RoomName));

            return Akka.Actor.Props.Create(() => new RoomSimulatorActor(
                RoomName,
                TemperatureSimulatorFactory.TemperatureSimulator(),
                CompanyPathBuilder.ActorPath()
            ));
        }
    }
}
