namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureDevice : ITemperatureDeviceDefinition, IDevice
    {
        double EffectivePower { get; }

        double DesiredTemperature { get; set; }

        ITemperatureMode ActiveMode { get; }

        void SetActiveMode(TemperatureModeType mode);
    }
}
