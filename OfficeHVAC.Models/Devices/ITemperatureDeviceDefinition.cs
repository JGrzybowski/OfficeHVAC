using System.Collections.Generic;

namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureDeviceDefinition
    {
        string Id { get; }

        int MaxPower { get; }

        IEnumerable<ITemperatureMode> Modes { get; }
    }
}