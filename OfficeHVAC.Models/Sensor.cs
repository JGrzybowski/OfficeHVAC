namespace OfficeHVAC.Models
{
    public class Sensor : ISensor
    {
        public string  Id { get; set; }
        public SensorTypes Type { get; set; }
    }
}
