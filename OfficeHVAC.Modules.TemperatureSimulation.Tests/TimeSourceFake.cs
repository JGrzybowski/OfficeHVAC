using NodaTime;
using OfficeHVAC.Models;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests
{
    public class TimeSourceFake : ITimeSource
    {
        public Instant Now { get; set; }

        public TimeSourceFake(Instant dateTime)
        {
            Now = dateTime;
        }
    }
}
