namespace OfficeHVAC.Modules.TemperatureSimulation.Factories
{
    public interface ITemperatureSimulatorFactory
    {
        double InitialTemperature { get; set; }

        ITemperatureSimulator TemperatureSimulator();
    }
}