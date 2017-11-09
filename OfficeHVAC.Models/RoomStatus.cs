﻿using NodaTime;
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

        public HashSet<IDevice> Devices { get; set; } = new HashSet<IDevice>();
        IEnumerable<IDevice> IRoomStatusMessage.Devices => Devices;

        public List<TemperatureJob> Jobs { get; set; } = new List<TemperatureJob>();
        IEnumerable<TemperatureJob> IRoomStatusMessage.Jobs => Jobs;

        public List<Requirements> Events { get; set; } = new List<Requirements>();
        IEnumerable<Requirements> IRoomStatusMessage.Events => Events;

        public IRoomStatusMessage ToMessage() => Clone();

        public RoomStatus Clone()
        {
            return new RoomStatus()
            {
                Id = Id,
                Volume = Volume,
                Name = Name,
                Parameters = Parameters.Clone(),
                Devices = new HashSet<IDevice>(Devices.Select(device => device.Clone())),
                Sensors = Sensors.Select(param => param.Clone() as ISensorActorRef).ToList(),

                Events = Events,
                Jobs = Jobs
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

        IEnumerable<IDevice> Devices { get; }

        IEnumerable<TemperatureJob> Jobs { get; }

        ParameterValuesCollection Parameters { get; }

        IEnumerable<Requirements> Events { get; }
    }
}