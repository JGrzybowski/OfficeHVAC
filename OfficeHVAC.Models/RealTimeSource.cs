using NodaTime;

namespace OfficeHVAC.Models
{
    public class RealTimeSource : ITimeSource
    {
        public Instant Now => SystemClock.Instance.Now;
    }
}
