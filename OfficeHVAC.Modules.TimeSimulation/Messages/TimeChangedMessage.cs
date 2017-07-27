using NodaTime;

namespace OfficeHVAC.Modules.TimeSimulation.Messages
{
    public class TimeChangedMessage
    {
        public Duration TimeDelta { get; }
        public Instant Now { get; }

        public TimeChangedMessage(Duration timeDelta, Instant now)
        {
            TimeDelta = timeDelta;
            Now = now;
        }
    }
}