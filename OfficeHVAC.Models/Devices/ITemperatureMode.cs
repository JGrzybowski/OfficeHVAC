namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureMode : IMode
    {
        TemperatureModeType Type { get; }

        IRange<double> TemperatureRange { get; set; }

        ITemperatureMode Clone();
    }
}
