using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Simulators;

namespace OfficeHVAC.Factories.Simulators.Temperature
{
    public class TemperatureSimulatorFactory : ITemperatureSimulatorFactory
    {
        private readonly ITimeSource timeSource;
        private readonly ITemperatureModel model;

        public float InitialTemperature { get; set; }

        public TemperatureSimulatorFactory(ITimeSource timeSource, ITemperatureModel model)
        {
            this.model = model;
            this.timeSource = timeSource;
        }

        public ITemperatureSimulator TemperatureSimulator()
        {
            return new TemperatureSimulator(timeSource, InitialTemperature, model)
            {
                Devices = new ITemperatureDevice[]
                {
                    new TemperatureDevice() { MaxPower = 2000 }
                }
            };
        }
    }
}
