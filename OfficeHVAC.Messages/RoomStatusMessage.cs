namespace OfficeHVAC.Messages
{
    public class RoomStatusMessage
    {
        public string RoomName { get; private set; }

        public double Temperature { get; private set; }

        public RoomStatusMessage(string roomName, double temperature)
        {
            RoomName = roomName;
            Temperature = temperature;
        }
    }
}
