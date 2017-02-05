using NodaTime;
using OfficeHVAC.Models.Devices;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Simulators
{
    public class SimpleTemperatureModel : ITemperatureModel
    {
        public double WattsToChangeOneDegreeInOneHour { get; set; } = 20;

        public virtual double CalculateChange(double temperature, IEnumerable<ITemperatureDevice> devices, Duration timeDelta)
        {
            var timespan = timeDelta.ToTimeSpan();
            var hoursSinceLastUpdate = timespan.TotalHours;
            return devices.Sum(device => device.EffectivePower * (device.DesiredTemperature - temperature))
                            / WattsToChangeOneDegreeInOneHour
                            * hoursSinceLastUpdate;
        }
    }
}
