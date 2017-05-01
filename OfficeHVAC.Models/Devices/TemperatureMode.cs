﻿namespace OfficeHVAC.Models.Devices
{
    public class TemperatureMode : ITemperatureMode
    {
        public string Name { get; set; }

        public double PowerConsumption { get; set; }

        public double PowerEfficiency { get; set; }

        public IRange<double> TemperatureRange { get; set; }

        public double CalculateEffectivePower(double maxPower) => maxPower * PowerConsumption * PowerEfficiency;

        public ITemperatureMode Clone() =>
            new TemperatureMode()
            {
                Name = Name,
                PowerConsumption = PowerConsumption,
                PowerEfficiency = PowerEfficiency,
                TemperatureRange = new Range<double>(TemperatureRange.Min, TemperatureRange.Max)
            };
    }
}
