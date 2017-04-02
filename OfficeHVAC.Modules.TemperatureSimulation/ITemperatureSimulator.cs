using System.Collections.Generic;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Modules.TemperatureSimulation
{
    public interface ITemperatureSimulator
    {
        double RoomVolume { get; set; }

        double GetTemperature(IRoomStatusMessage status);

        void SetTemperature(double value);

        ITemperatureModel Model { get; }

        ITimeSource TimeSource { get; }
    }
}