namespace OfficeHVAC.Messages
{
    public class RemoveDevice
    {
        public string Id { get; }

        public RemoveDevice(string id)
        {
            Id = id;
        }
    }
}
