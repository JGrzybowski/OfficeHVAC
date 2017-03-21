using System.Collections.Generic;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Modules.TemperatureSimulation
{
    public interface ITemperatureSimulator
    {
        double RoomVolume { get; set; }

        double Temperature { get; set; }

        ITemperatureModel Model { get; }

        IEnumerable<ITemperatureDevice> Devices { get; set; }

        ITimeSource TimeSource { get; }
    }
}