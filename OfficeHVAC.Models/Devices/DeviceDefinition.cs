namespace OfficeHVAC.Models.Devices
{
    public class DeviceDefinition
    {
        public string Id { get; set; }

        public bool IsTurnedOn { get; }

        public int MaxPower { get; set; }
    }
}
