namespace OfficeHVAC.Messages
{
    public class RoomStatusMessage
    {
        public string RoomName { get; private set; }
        public float Temperature { get; private set; }

        public RoomStatusMessage(string roomName, float temperature)
        {
            this.RoomName = roomName;
            this.Temperature = temperature;
        }
    }
}
