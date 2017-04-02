using NodaTime;
using System;
using System.Collections.Generic;
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
            Modes = new HashSet<ITemperatureMode>() { offMode };
            this.SetActiveModeByName(offMode.Name);
        }

        public string Id { get; set; }

        public enum Mode { Off, StandBy, Eco, Normal, Turbo, Stabilization }

        public bool IsTurnedOn => PowerConsumption > 0;

        public int MaxPower { get; set; }

        public ITemperatureMode ActiveMode { get; private set; }

        public IDevice Clone()
        {
            TemperatureDevice clone = new TemperatureDevice
            {
                Id = this.Id,
                DesiredTemperature = this.DesiredTemperature,
                Modes = this.Modes.Select(m => m.Clone()).ToList(),
                MaxPower = this.MaxPower
            };
            clone.ActiveMode = clone.Modes.Single(m => m.Name == this.ActiveMode.Name);
            return clone;
        }

        public double EffectivePower => this.ActiveMode.CalculateEffectivePower(MaxPower);

        public double PowerConsumption => MaxPower * Math.Abs(EffectivePower);

        public void TurnOff() => SetActiveModeByName(nameof(Mode.Off));

        public string GetActiveModeByName()
        {
            return ActiveMode.Name;
        }

        public void SetActiveModeByName(string value)
        {
            if (Modes.Any(m => m.Name == value))
            {
                ActiveMode = Modes.Single(m => m.Name == value);
            }
            else
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "Provided mode name is not a valid one. Use Modes property to find out valid Mode Names");
        }

        public IReadOnlyCollection<string> ModesNames => Modes.Select(mode => mode.Name).ToList();
        public ICollection<ITemperatureMode> Modes { get; set; }

        public double DesiredTemperature { get; set; }

        public double CalculatePowerConsumption(string name, Duration time) =>
            this.Modes.Single(m => m.Name == name)
                .CalculateEffectivePower(MaxPower) * time.ToTimeSpan().TotalSeconds;

        IEnumerable<ITemperatureMode> ITemperatureDeviceDefinition.Modes => this.Modes;
    }
}
