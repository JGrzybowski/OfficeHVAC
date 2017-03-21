namespace OfficeHVAC.Models.Devices
{
    public interface IDevice
    {
        bool IsTurnedOn { get; }

        void TurnOff();

        double PowerConsumption { get; }

        int MaxPower { get; set; }
    }
}
