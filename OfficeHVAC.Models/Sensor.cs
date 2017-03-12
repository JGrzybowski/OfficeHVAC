namespace OfficeHVAC.Models
{
    public class Sensor : ISensor
    {
        public string  Id { get; set; }
        public SensorType Type { get; set; }
    }
}
