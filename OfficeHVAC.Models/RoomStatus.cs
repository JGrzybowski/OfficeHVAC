using OfficeHVAC.Models.Devices;
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

        public HashSet<IDevice> Devices { get; set; } = new HashSet<IDevice>();
        IEnumerable<IDevice> IRoomStatusMessage.Devices => this.Devices;

        public List<TemperatureJob> Jobs { get; set; } = new List<TemperatureJob>();
        IEnumerable<TemperatureJob> IRoomStatusMessage.Jobs => this.Jobs;

        public List<Requirements> Events { get; set; } = new List<Requirements>();
        IEnumerable<Requirements> IRoomStatusMessage.Events => this.Events;

        public IRoomStatusMessage ToMessage() => this.Clone() as IRoomStatusMessage;

        public object Clone()
        {
            return new RoomStatus()
            {
                Id = Id,
                Volume = Volume,
                Name = Name,
                Parameters = Parameters.Clone(),
                Devices = new HashSet<IDevice>(this.Devices.Select(device => device.Clone())),
                Sensors = this.Sensors.Select(param => param.Clone() as ISensorActorRef).ToList(),

                Events = this.Events,
                Jobs = this.Jobs
            };
        }

        public class Request { }
    }

    public interface IRoomStatusMessage : ICloneable
    {
        string Id { get; }

        string Name { get; }

        double Volume { get; }

        IEnumerable<IDevice> Devices { get; }

        IEnumerable<TemperatureJob> Jobs { get; }

        ParameterValuesCollection Parameters { get; }

        IEnumerable<Requirements> Events { get; }
    }
}