using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeHVAC.Models.Devices
{
    public class DeviceDefinition
    {
        public string Id { get; set; }

        public bool IsTurnedOn { get; }

        public int MaxPower { get; set; }
    }
}
