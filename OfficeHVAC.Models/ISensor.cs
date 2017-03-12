using System;

namespace OfficeHVAC.Models
{
    public interface ISensor : ICloneable
    {
        string Id { get; set; }
        SensorType Type { get; set; }
    }
}