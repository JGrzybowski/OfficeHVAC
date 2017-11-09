using NodaTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OfficeHVAC.Models.Devices
{
    public class TemperatureDevice : ITemperatureDevice
    {
        public TemperatureDevice()
        {
            var offMode = new TemperatureMode()
            {
                Name = "Off",
                PowerConsumption = 0,
                PowerEfficiency = 0,
                TemperatureRange = new Range<double>(double.NegativeInfinity, double.PositiveInfinity)
            };
            Modes = new ModesCollection() { offMode };
            SetActiveMode(TemperatureModeType.Off);
        }

        public string Id { get; set; }

        public bool IsTurnedOn => PowerConsumption > 0;

        public int MaxPower { get; set; }

        private TemperatureModeType activeMode;
        public ITemperatureMode ActiveMode => Modes[activeMode];

        public IDevice Clone()
        {
            TemperatureDevice clone = new TemperatureDevice
            {
                Id = Id,
                DesiredTemperature = DesiredTemperature,
                Modes = new ModesCollection(Modes.Select(m => m.Clone())),
                MaxPower = MaxPower
            };
            clone.SetActiveMode(ActiveMode.Type);
            return clone;
        }

        public double EffectivePower => ActiveMode.CalculateEffectivePower(MaxPower);

        public double PowerConsumption => MaxPower * Math.Abs(EffectivePower);

        public void TurnOff() => SetActiveMode(TemperatureModeType.Off);

        public void SetActiveMode(TemperatureModeType value)
        {
            if (Modes.Contains(value))
                activeMode = value;
            else
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "Provided mode type is not a valid one. Use Modes property to find out avaliable mode tpyes");
        }

        public KeyedCollection<TemperatureModeType, ITemperatureMode> Modes { get; set; }

        public double DesiredTemperature { get; set; }

        public double CalculatePowerConsumption(TemperatureModeType modeType, Duration time) =>
            Modes[modeType].CalculateEffectivePower(MaxPower) * time.ToTimeSpan().TotalSeconds;

        IEnumerable<ITemperatureMode> ITemperatureDeviceDefinition.Modes => Modes;
    }
}
