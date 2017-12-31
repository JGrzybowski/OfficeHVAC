using NodaTime;

namespace OfficeHVAC.Messages
{
    public class TimeChangedMessage
    {
        public Instant Now { get; }

        public TimeChangedMessage(Instant now)
        {
            Now = now;
        }
    }
}