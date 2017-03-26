using System.Collections.Generic;
using System.Linq;

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
    }
}
