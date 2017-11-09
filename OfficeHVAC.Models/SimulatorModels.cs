using Akka.Actor;

namespace OfficeHVAC.Models
{
    public class SimulatorModels : ISimulatorModels
    {
        public IActorRef TimeSource => timeSource;
        private IActorRef timeSource;

        public ITemperatureModel TemperatureModel { get; }

        public SimulatorModels(IActorRef timeSource, ITemperatureModel temperatureModel)
        {
            this.timeSource = timeSource;
            this.TemperatureModel = temperatureModel;
        }
    }
}
