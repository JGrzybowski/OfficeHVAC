using NodaTime;
using OfficeHVAC.Models;

namespace OfficeHVAC.Modules.TimeSimulation.TimeSources
{
    public interface IControlledTimeSource : ITimeSource
    {
        double Speed { get; set; }

        void AddHours(long hours);
        void AddMinutes(long minutes);
        void AddSeconds(long seconds);

        void Reset(Instant instant);
        void UpdateClock();
    }
}