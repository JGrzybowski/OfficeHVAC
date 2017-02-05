using NodaTime;
using OfficeHVAC.Models.Devices;
using System.Collections.Generic;

namespace OfficeHVAC.Simulators
{
    public interface ITemperatureModel
    {
        double CalculateChange(double temperature, IEnumerable<ITemperatureDevice> devices, Duration timeDelta);
    }
}
