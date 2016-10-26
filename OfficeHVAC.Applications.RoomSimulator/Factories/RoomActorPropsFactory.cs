using System;
using Akka.Actor;
using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Factories.Propses;
using OfficeHVAC.Factories.Simulators.Temperature;
using OfficeHVAC.Factories.TimeSources;
using Prism.Mvvm;

namespace OfficeHVAC.Applications.RoomSimulator.Factories
{
    public class RoomActorPropsFactory : BindableBase, IPropsFactory
    {
        public RoomActorPropsFactory(IActorPathBuilder companyPathBuilder, ITimeSourceFactory timeSourceFactory, ITemperatureSimulatorFactory temperatureSimulatorFactory)
        {
            this.CompanyPathBuilder = companyPathBuilder;
            this.TimeSourceFactory = timeSourceFactory;
            this.TemperatureSimulatorFactory = temperatureSimulatorFactory;
        }

        public float Temperature { get; set; }

        private string _roomName;
        public string RoomName
        {
            get { return _roomName; }
            set { SetProperty(ref _roomName, value); }
        }

        public IActorPathBuilder CompanyPathBuilder { get; private set; }
        public ITimeSourceFactory TimeSourceFactory { get; private set; }
        public ITemperatureSimulatorFactory TemperatureSimulatorFactory { get; private set; }

        public Props Build()
        {
            throw new NotImplementedException();
        }
    }
}
