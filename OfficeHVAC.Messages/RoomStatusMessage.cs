using OfficeHVAC.Models;

namespace OfficeHVAC.Messages
{
    public class RoomStatusMessage
    {
        public string RoomName { get; private set; }

        public double Temperature { get; private set; }

        public RoomStatusMessage(RoomInfo roomInfo, double temperature)
        {
            RoomName = roomInfo.Name;
            Temperature = temperature;
        }
    }
}
