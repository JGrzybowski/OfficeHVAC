using OfficeHVAC.Models;
using OfficeHVAC.Simulators;

namespace OfficeHVAC.Factories.Simulators.Temperature
{
    public class TemperatureSimulatorFactory : ITemperatureSimulatorFactory
    {
        private readonly ITimeSource timeSource;
        public float InitialTemperature { get; set; }

        public TemperatureSimulatorFactory(ITimeSource timeSource)
        {
            this.timeSource = timeSource;
        }

        public ITemperatureSimulator TemperatureSimulator()
        {
            return new TemperatureSimulator(timeSource, InitialTemperature);
        }
    }
}
