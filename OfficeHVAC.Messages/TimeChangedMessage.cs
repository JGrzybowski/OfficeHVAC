using NodaTime;

namespace OfficeHVAC.Modules.TimeSimulation.Messages
{
    public class TimeChangedMessage
    {
        public Instant Now { get; }

        public TimeChangedMessage(Duration timeDelta, Instant now)
        {
            Now = now;
        }
        
        public TimeChangedMessage(Instant now)
        {
            Now = now;
        }
    }
}