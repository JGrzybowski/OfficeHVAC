using Akka.Actor;
using OfficeHVAC.Actors;
using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Factories.Propses;
using OfficeHVAC.Factories.Simulators.Temperature;
using Prism.Mvvm;

namespace OfficeHVAC.Applications.RoomSimulator.Factories
{
    public class RoomSimulatorActorPropsFactory : BindableBase, IRoomSimulatorActorPropsFactory
    {
        public RoomSimulatorActorPropsFactory(IActorPathBuilder companyPathBuilder, ITemperatureSimulatorFactory temperatureSimulatorFactory)
        {
            this.CompanyPathBuilder = companyPathBuilder;
            this.TemperatureSimulatorFactory = temperatureSimulatorFactory;
        }

        public float Temperature { get; set; }

        private string _roomName;
        public string RoomName
        {
            get { return _roomName; }
            set { SetProperty(ref _roomName, value); }
        }

        public IActorPathBuilder CompanyPathBuilder { get; }
        public ITemperatureSimulatorFactory TemperatureSimulatorFactory { get; }

        public Props Props()
        {
            return Akka.Actor.Props.Create(() => new RoomSimulatorActor(
                this.RoomName,
                this.TemperatureSimulatorFactory.TemperatureSimulator(),
                this.CompanyPathBuilder.ActorPath()
            ));
        }
    }
}
