using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureDevice : IDevice
    {
        int MaxPower { get; set; }
        float TemperatureChage { get; set; }
    }
}
