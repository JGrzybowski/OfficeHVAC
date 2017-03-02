namespace OfficeHVAC.Modules.TemperatureSimulation.Factories
{
    public interface ITemperatureSimulatorFactory
    {
        float InitialTemperature { get; set; }

        ITemperatureSimulator TemperatureSimulator();
    }
}