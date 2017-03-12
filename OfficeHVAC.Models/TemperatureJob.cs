using NodaTime;

namespace OfficeHVAC.Models
{
    public class TemperatureJob
    {
        public TemperatureJob(string modeName, Instant startTime, Instant endTime)
        {
            ModeName = modeName;
            StartTime = startTime;
            EndTime = endTime;
        }

        public string ModeName { get; }

        public Instant StartTime { get; }

        public Instant EndTime { get; }

        public Duration Duration => EndTime - StartTime;
    }
}
