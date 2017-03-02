using OfficeHVAC.Models.Devices;
using System.Collections.Generic;

namespace OfficeHVAC.Models
{
    public interface IParameterSimulator
    {
        IEnumerable<ITemperatureDevice> Devices { get; set; }
        ITimeSource TimeSource { get; }
    }
}
