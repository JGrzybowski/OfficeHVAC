using OfficeHVAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Messages
{
    public class AddDevice<TDeviceDefinition>
    {
        public TDeviceDefinition Definition;

        public AddDevice(TDeviceDefinition definition)
        {
            Definition = definition;
        }
    }

    public class AddTemperatureDevice : AddDevice<ITemperatureDeviceDefinition>
    {
        public AddTemperatureDevice(ITemperatureDeviceDefinition definition) : base(definition) { }
    }
}
