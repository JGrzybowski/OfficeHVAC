using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Models
{
    public class RoomStatus : IRoomStatusMessage, IToMessage<IRoomStatusMessage>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Volume { get; set; }

        public IEnumerable<ISensorActorRef> Sensors { get; set; } = new HashSet<ISensorActorRef>();

        public ParameterValuesCollection Parameters { get; set; } = new ParameterValuesCollection();

        public IRoomStatusMessage ToMessage() => this.Clone() as IRoomStatusMessage;

        public object Clone()
        {
            return new RoomStatus()
            {
                Id = Id,
                Volume = Volume,
                Name = Name,
                Parameters = Parameters.Clone(),
                Sensors = this.Sensors.Select(param => param.Clone() as ISensorActorRef).ToList()
            };
        }

        public class Request { }
    }

    public interface IRoomStatusMessage : ICloneable
    {
        string Id { get; set; }

        string Name { get; set; }

        double Volume { get; set; }

        ParameterValuesCollection Parameters { get; }
    }
}
