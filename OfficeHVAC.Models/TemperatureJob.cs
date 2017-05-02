using System;
using System.Collections.Generic;
using NodaTime;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Models
{
    public class TemperatureJob
    {
        public TemperatureJob(TemperatureModeType temperatureModeType, double desiredTemperature, Instant startTime, Instant endTime)
        {
            ModeType = temperatureModeType;
            DesiredTemperature = desiredTemperature;
            StartTime = startTime;
            EndTime = endTime;
        }

        public TemperatureModeType ModeType { get; }

        public double DesiredTemperature { get; }

        public Instant StartTime { get; }

        public Instant EndTime { get; }

        public Duration Duration => EndTime - StartTime;

        public class StartDateComparer : IComparer<TemperatureJob>
        {
            public int Compare(TemperatureJob x, TemperatureJob y)
            {
                return Math.Sign((x.StartTime - y.StartTime).Ticks);
            }
        }
    }
}
