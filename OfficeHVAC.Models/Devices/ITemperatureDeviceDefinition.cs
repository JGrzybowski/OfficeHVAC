using System.Collections.Generic;
using NodaTime;

namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureDeviceDefinition
    {
        string Id { get; }

        int MaxPower { get; }

        IEnumerable<ITemperatureMode> Modes { get; }

        double CalculatePowerConsumption(TemperatureModeType modeType, Duration time);
    }
}