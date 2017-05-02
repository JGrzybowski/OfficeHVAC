using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace OfficeHVAC.Models.Devices
{
    public class TemperatureDeviceDefinition : DeviceDefinition, ITemperatureDeviceDefinition, IToMessage<ITemperatureDeviceDefinition>
    {
        public IEnumerable<ITemperatureMode> Modes { get; set; }

        private TemperatureDeviceDefinition Clone()
        {
            var clone = new TemperatureDeviceDefinition()
            {
                Id = Id,
                MaxPower = MaxPower,
                Modes = Modes.Select(m => m.Clone())
            };
            return clone;
        }

        public ITemperatureDeviceDefinition ToMessage() => Clone();

        public double CalculatePowerConsumption(TemperatureModeType modeType, Duration time) =>
            this.Modes.Single(m => m.Type == modeType)
                .CalculateEffectivePower(MaxPower) * time.ToTimeSpan().TotalSeconds;
    }
}
