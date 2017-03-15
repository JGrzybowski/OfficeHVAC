using OfficeHVAC.Models;

namespace OfficeHVAC.Modules.TemperatureSimulation
{
    public interface ITemperatureSimulator : IParameterSimulator
    {
        double RoomVolume { get; set; }

        double Temperature { get; set; }

        ITemperatureModel Model { get; }
    }
}