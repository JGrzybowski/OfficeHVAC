﻿using NodaTime;
using OfficeHVAC.Models.Devices;
using System.Collections.Generic;

namespace OfficeHVAC.Models
{
    public interface ITemperatureModel
    {
        double CalculateChange(double temperature, IEnumerable<ITemperatureDevice> devices, Duration timeDelta, double volume);

        Duration CalculateNeededTime(double initialTemperature, double desiredTemperature, IEnumerable<ITemperatureDevice> devices, string mode, double volume);
    }
}
