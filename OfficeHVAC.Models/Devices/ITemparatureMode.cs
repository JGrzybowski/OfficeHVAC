namespace OfficeHVAC.Models.Devices
{
    public interface ITemparatureMode : IMode
    {
        IRange<double> TemperatureRange { get; set; }
    }
}