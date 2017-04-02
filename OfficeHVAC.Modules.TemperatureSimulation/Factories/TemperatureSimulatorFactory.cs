using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Modules.TemperatureSimulation.Factories
{
    public class TemperatureSimulatorFactory : ITemperatureSimulatorFactory
    {
        private readonly ITimeSource timeSource;
        private readonly ITemperatureModel model;

        public double InitialTemperature { get; set; }

        public TemperatureSimulatorFactory(ITimeSource timeSource, ITemperatureModel model)
        {
            this.model = model;
            this.timeSource = timeSource;
        }

        public ITemperatureSimulator TemperatureSimulator()
        {
            return new TemperatureSimulator(timeSource, InitialTemperature, model);
        }
    }
}
