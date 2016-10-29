using System.Collections.Generic;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Simulators
{
    public interface IParameterSimulator
    {
        IEnumerable<ITemperatureDevice> Devices { get; set; }
        ITimeSource TimeSource { get; }
    }
}
