namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureMode : IMode
    {
        IRange<double> TemperatureRange { get; set; }
    }
}