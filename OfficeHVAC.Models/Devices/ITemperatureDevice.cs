using System.Collections.Generic;

namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureDevice : IDevice
    {
        double EffectivePower { get; }

        double DesiredTemperature { get; set; }
        
        string SetActiveModeByName { get; set; }

        IReadOnlyCollection<string> ModesNames { get; }

        ICollection<ITemperatureMode> Modes { get; set; }
    }
}
