using System;
using System.Collections.Generic;
using NodaTime;

namespace OfficeHVAC.Models
{
    public class TemperatureJob
    {
        public TemperatureJob(string modeName, double desiredTemperature, Instant startTime, Instant endTime)
        {
            ModeName = modeName;
            DesiredTemperature = desiredTemperature;
            StartTime = startTime;
            EndTime = endTime;
        }

        public string ModeName { get; }

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
