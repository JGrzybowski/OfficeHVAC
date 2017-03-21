using System;

namespace OfficeHVAC.Models
{
    public class RoomInfo : ICloneable
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Area { get; set; }

        public object Clone() => new RoomInfo()
        {
            Area = Area,
            Id = Id,
            Name = Name
        };
    }
}
