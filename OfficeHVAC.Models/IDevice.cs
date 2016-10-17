namespace OfficeHVAC.Models
{
    public interface IDevice
    {
        bool IsTurnedOn { get; }
        byte UsedPower { get; set; }
        void TurnOff();
    }
}
