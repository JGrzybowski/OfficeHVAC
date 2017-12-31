using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace OfficeHVAC.Models.Devices
{
    public class TemperatureDeviceDefinition : DeviceDefinition, ITemperatureDeviceDefinition, IToMessage<ITemperatureDeviceDefinition>
    {
        public IEnumerable<ITemperatureMode> Modes { get; set; } = new List<ITemperatureMode>();
        
        public TemperatureDeviceDefinition(string id, int maxPower) : base(id, maxPower) { }
        public TemperatureDeviceDefinition(string id, int maxPower, IEnumerable<ITemperatureMode> modes) : base(id, maxPower)
        {
            Modes = modes.Select(m => m.Clone()).ToArray();
        }
        
        private TemperatureDeviceDefinition Clone() => new TemperatureDeviceDefinition(Id, MaxPower, Modes);
        public ITemperatureDeviceDefinition ToMessage() => Clone();

        public double CalculatePowerConsumption(TemperatureModeType modeType, Duration time) =>
            Modes.Single(m => m.Type == modeType).CalculateEffectivePower(MaxPower) * time.ToTimeSpan().TotalSeconds;
    }
}
