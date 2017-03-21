using NodaTime;
using OfficeHVAC.Models;

namespace OfficeHVAC.Modules.TimeSimulation.TimeSources
{
    public class RealTimeSource : ITimeSource
    {
        public Instant Now => SystemClock.Instance.Now;
    }
}
