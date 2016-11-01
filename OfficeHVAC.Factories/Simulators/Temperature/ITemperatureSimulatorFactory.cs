using OfficeHVAC.Simulators;

namespace OfficeHVAC.Factories.Simulators.Temperature
{
    public interface ITemperatureSimulatorFactory
    {
        float InitialTemperature { get; set; }

        ITemperatureSimulator TemperatureSimulator();
    }
}