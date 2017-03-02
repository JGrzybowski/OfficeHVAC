using OfficeHVAC.Models;

namespace OfficeHVAC.Modules.TemperatureSimulation
{
    public interface ITemperatureSimulator : IParameterSimulator
    {
        double Temperature { get; set; }

        ITemperatureModel Model { get; }
    }
}