using System.Collections.Generic;

namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureDevice : IDevice
    {
        float HeatingParameter { get; set; }
        double DesiredTemperature { get; set; }

        IReadOnlyCollection<string> Modes { get; }
        IReadOnlyDictionary<string, double> EstimatedPowerConsumption { get; }
    }
}
