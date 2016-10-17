namespace OfficeHVAC.Models.Devices
{
    public interface IDevice
    {
        bool IsTurnedOn { get; }
        double PowerConsumption { get; }
        void TurnOff();
    }
}
