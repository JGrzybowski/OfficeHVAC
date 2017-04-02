using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeHVAC.Models
{
    public interface ISimulatorModels
    {
        ITimeSource TimeSource { get; }

        ITemperatureModel TemperatureModel { get; }
    }
}
