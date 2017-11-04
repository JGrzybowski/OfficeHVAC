using OfficeHVAC.Models;

namespace OfficeHVAC.Messages {
    public class SensorAvaliableMessage
    {
        public SensorType SensorType { get; }
        public string SensorId { get; }

        public SensorAvaliableMessage(SensorType sensorType, string sensorId)
        {
            SensorType = sensorType;
            SensorId = sensorId;
        }
    }
}