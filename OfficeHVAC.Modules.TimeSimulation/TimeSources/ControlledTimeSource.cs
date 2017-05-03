using NodaTime;
using System;
using TimeSimulator.NodaTime;

namespace OfficeHVAC.Modules.TimeSimulation.TimeSources
{
    public class ControlledTimeSource : NodaTimeControllableClock, IControlledTimeSource
    {
        private static readonly long TicksInSecond = Duration.FromSeconds(1).BclCompatibleTicks;

        private double _speed = 1;
        public double Speed
        {
            get { return _speed; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Speed cannot have negative value");
                _speed = value;
            }
        }

        public ControlledTimeSource(Instant initial)
        {
            Now = initial;
        }

        public void AddHours(int hours) => AddInterval(Duration.FromHours(hours));

        public void AddMinutes(long minutes) => AddInterval(Duration.FromMinutes(minutes));

        public void AddSeconds(long seconds) => AddInterval(Duration.FromSeconds(seconds));

        public void UpdateClock()
        {
            var delta = (long)(TicksInSecond * Speed);
            AddInterval(Duration.FromTicks(delta));
        }

        public void Reset(Instant instant) => Now = instant;
    }
}
