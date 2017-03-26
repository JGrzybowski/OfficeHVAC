namespace OfficeHVAC.Models.Devices
{
    public interface IDevice
    {
        string Id { get; }

        bool IsTurnedOn { get; }

        void TurnOff();

        double PowerConsumption { get; }

        int MaxPower { get; set; }
    }
}
