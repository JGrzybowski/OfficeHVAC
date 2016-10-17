namespace OfficeHVAC.Models.Devices
{
    public interface IDevice
    {
        bool IsTurnedOn { get; }
        double PowerConsumption { get; set; }
        void TurnOff();
    }
}
