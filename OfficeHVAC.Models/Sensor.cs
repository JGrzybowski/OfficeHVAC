namespace OfficeHVAC.Models
{
    public class Sensor : ISensor
    {
        public string  Id { get; set; }
        public SensorType Type { get; set; }
        public object Clone()
        {
            return new Sensor()
            {
                Id = this.Id,
                Type = this.Type
            };
        }
    }
}
