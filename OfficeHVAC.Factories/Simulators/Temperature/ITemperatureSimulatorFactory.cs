using OfficeHVAC.Simulators;

namespace OfficeHVAC.Factories.Simulators.Temperature
{
    public interface ITemperatureSimulatorFactory
    {
        ITemperatureSimulator TemperatureSimulator();
    }
}