namespace OfficeHVAC.Models
{
    public interface ISensor
    {
        string Id { get; set; }
        SensorTypes Type { get; set; }
    }
}