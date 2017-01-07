using NodaTime;
using OfficeHVAC.Models;

namespace OfficeHVAC.Simulators.Tests
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
