namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureDevice : IDevice
    {
        int MaxPower { get; set; }
        float HeatingParameter { get; set; }
    }
}
