namespace OfficeHVAC.Modules.TemperatureSimulation.Messages {
    public class AddTemperatureActuatorMessage
    {
        public string Id { get; }
        public AddTemperatureActuatorMessage(string id)
        {
            Id = id;
        }
    }
}