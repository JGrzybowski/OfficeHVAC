using System;
using MoreLinq;
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
        private const double AirsDensity = 1.200;        // kg / m^3

        public virtual double CalculateChange(double temperature, IEnumerable<ITemperatureDevice> devices, Duration timeDelta, double volume)
        {
            var seconds = timeDelta.ToTimeSpan().TotalSeconds;
            var power = devices.Sum(device => device.EffectivePower * (device.DesiredTemperature - temperature));

            return (power * seconds) / (AirsDensity * volume * AirsSpecificHeat);
        }

        public Duration CalculateNeededTime(double initialTemperature, double desiredTemperature, IEnumerable<ITemperatureDeviceDefinition> devices, string mode, double volume)
        {
            var deltaT = Math.Abs(desiredTemperature - initialTemperature);
            var power = devices.Sum(dev => dev.Modes.Single(m => m.Name == mode).CalculateEffectivePower(dev.MaxPower));
            if (power == 0)
                return Duration.FromTimeSpan(TimeSpan.MaxValue);

            var seconds = AirsSpecificHeat * AirsDensity * volume * deltaT / power;

            var time = Duration.FromSeconds((long)seconds);
            return time;
        }

        public string FindMostEfficientCombination(TemperatureTask task, IRoomStatusMessage status, IEnumerable<ITemperatureDeviceDefinition> devices)
        {
            var modesNames = devices.Select(dev => dev.Modes.Select(d => d.Name))
                                    .Aggregate((resultModes, newModes) => resultModes.Intersect(newModes));

            var bestMode = modesNames
                .Select(modeName => new
                {
                    Time = CalculateNeededTime(task.InitialTemperature, task.DesiredTemperature, devices, modeName, status.Volume),
                    ModeName = modeName
                })
                .Where(timeMode => (timeMode.Time < task.TimeLimit))
                .Select(timeMode => new
                {
                    Power = devices.Sum(dev => dev.CalculatePowerConsumption(timeMode.ModeName, timeMode.Time)),
                    timeMode.ModeName
                })
                .MinBy(powerMode => powerMode.Power);
            return bestMode.ModeName;
        }
    }
}
