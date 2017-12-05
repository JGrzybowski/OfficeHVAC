using NodaTime;
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

        public Instant TimeStamp { get; set; }

        public IEnumerable<ISensorActorRef> Sensors { get; set; } = new HashSet<ISensorActorRef>();

        public ParameterValuesCollection Parameters { get; set; } = new ParameterValuesCollection();

        public HashSet<ITemperatureDeviceStatus> TemperatureDevices { get; set; } = new HashSet<ITemperatureDeviceStatus>();
        IEnumerable<ITemperatureDeviceStatus> IRoomStatusMessage.TemperatureDevices => TemperatureDevices;

        public List<Requirement<double>> Events { get; set; } = new List<Requirement<double>>();
        IEnumerable<Requirement<double>> IRoomStatusMessage.Events => Events;

        public IRoomStatusMessage ToMessage() => Clone();

        public RoomStatus Clone()
        {
            return new RoomStatus()
            {
                Id = Id,
                Volume = Volume,
                Name = Name,
                Parameters = Parameters.Clone(),

                TemperatureDevices = new HashSet<ITemperatureDeviceStatus>(TemperatureDevices),
                Sensors = Sensors.Select(param => param.Clone() as ISensorActorRef).ToList(),
                
                Events = Events
            };
        }

        object ICloneable.Clone() => Clone();

        public class Request { }
    }

    public interface IRoomStatusMessage : ICloneable
    {
        string Id { get; }

        string Name { get; }

        double Volume { get; }

        Instant TimeStamp { get; }

        IEnumerable<ITemperatureDeviceStatus> TemperatureDevices { get; }

        IEnumerable<Requirement<double>> Events { get; }

        ParameterValuesCollection Parameters { get; }
    }
}