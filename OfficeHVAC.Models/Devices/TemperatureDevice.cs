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
            Modes  = new HashSet<ITemperatureMode>() { offMode };
            this.SetActiveModeByName = offMode.Name;
        }

        public enum Mode { Off, StandBy, Eco, Normal, Turbo }

        public bool IsTurnedOn => PowerConsumption > 0;

        public int MaxPower { get; set; }

        public double EffectivePower => this.ActiveMode.CalculateEffectivePower(MaxPower);

        public double PowerConsumption => MaxPower * Math.Abs(EffectivePower);

        public void TurnOff() => SetActiveModeByName = nameof(Mode.Off);

        public ITemperatureMode ActiveMode { get; private set; }

        public string SetActiveModeByName
        {
            get { return ActiveMode.Name; }
            set
            {
                if (Modes.Any(m => m.Name == value))
                {
                    ActiveMode = Modes.Single(m => m.Name == value);
                }
                else 
                    throw new ArgumentOutOfRangeException(nameof(value), value, 
                        "Provided mode name is not a valid one. Use Modes property to find out valid Mode Names");
            }
        }

        public IReadOnlyCollection<string> ModesNames => Modes.Select(mode => mode.Name).ToList();
        public ICollection<ITemperatureMode> Modes { get; set; } 

        public double DesiredTemperature { get; set; }
    }
}
