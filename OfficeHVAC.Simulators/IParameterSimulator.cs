using System.Collections.Generic;
using OfficeHVAC.Models;

namespace OfficeHVAC.Simulators
{
    public interface IParameterSimulator
    {
        IEnumerable<IDevice> Devices { get; set; }
        ITimeSource TimeSource { get; set; }
    }
}
