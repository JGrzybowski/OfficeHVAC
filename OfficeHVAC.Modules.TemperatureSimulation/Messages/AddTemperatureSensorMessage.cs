namespace OfficeHVAC.Modules.TemperatureSimulation.Messages
{
    public class AddTemperatureSensorMessage
    {
        public string TemperatureParamerersActorPath { get; }
        public string SensorId { get; }
        public string TimeActorPath { get; }
     
        public AddTemperatureSensorMessage(string timeActorPath, string temperatureParamerersActorPath, string sensorId)
        {
            TemperatureParamerersActorPath = temperatureParamerersActorPath;
            SensorId = sensorId;
            TimeActorPath = timeActorPath;
        }
    }
}