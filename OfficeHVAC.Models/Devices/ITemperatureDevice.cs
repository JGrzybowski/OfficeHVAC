using NodaTime;
using System.Collections.Generic;

namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureDevice : ITemperatureDeviceDefinition, IDevice
    {
        double EffectivePower { get; }

        double DesiredTemperature { get; set; }

        string GetActiveModeByName();
        void SetActiveModeByName(string value);

        IReadOnlyCollection<string> ModesNames { get; }

        ICollection<ITemperatureMode> Modes { get; set; }

        double CalculatePowerConsumption(string name, Duration time);
    }
}
