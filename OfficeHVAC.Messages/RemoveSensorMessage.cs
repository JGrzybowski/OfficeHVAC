namespace OfficeHVAC.Messages
{
    public class RemoveSensorMessage
    {
        public string Id { get; }
        
        public RemoveSensorMessage(string id)
        {
            Id = id;
        }
    }
}