namespace OfficeHVAC.Models.Devices
{
    public class DeviceDefinition
    {
        public DeviceDefinition(string id, int maxPower)
        {
            Id = id;
            MaxPower = maxPower;
        }
        public string Id { get; }

        public int MaxPower { get; }
    }
}
