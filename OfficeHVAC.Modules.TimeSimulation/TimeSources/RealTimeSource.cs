using NodaTime;

namespace OfficeHVAC.Models.TimeSources
{
    public class RealTimeSource : ITimeSource
    {
        public Instant Now => SystemClock.Instance.Now;
    }
}
