using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Modules.TemperatureSimulation
{
    public class SimpleTemperatureModel : ITemperatureModel
    {
        private const double AirsSpecificHeat = 1005;   //  W / kg*K*s
        private const double AirsDensity = 1200;        // kg / m^3

        public virtual double CalculateChange(double temperature, IEnumerable<ITemperatureDevice> devices, Duration timeDelta, double volume)
        {
            var seconds = timeDelta.ToTimeSpan().TotalSeconds;
            var power = devices.Sum(device => device.EffectivePower * (device.DesiredTemperature - temperature));

            return (power * seconds) / (AirsDensity * volume * AirsSpecificHeat);
        }

        public Duration CalculateNeededTime(double initialTemperature, double desiredTemperature, IEnumerable<ITemperatureDevice> devices, string mode, double volume)
        {
            var deltaT = desiredTemperature - initialTemperature;
            var power = devices.Sum(dev => dev.Modes.Single(m => m.Name == mode).CalculateEffectivePower(dev.MaxPower));

            var seconds = AirsSpecificHeat * AirsDensity * volume * deltaT / power;

            var time = Duration.FromSeconds((long) seconds);
            return time;
        }
    }
}
