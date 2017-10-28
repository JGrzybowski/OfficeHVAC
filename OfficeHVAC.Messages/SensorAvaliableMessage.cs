using OfficeHVAC.Models;

namespace OfficeHVAC.Messages {
    public class SensorAvaliableMessage
    {
        public SensorType SensorType { get; }
        public string SendorId { get; }

        public SensorAvaliableMessage(SensorType sensorType, string sendorId)
        {
            SensorType = sensorType;
            SendorId = sendorId;
        }
    }
}