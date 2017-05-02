using NodaTime;
using NodaTime.Testing;
using System;

namespace OfficeHVAC.Modules.TimeSimulation.TimeSources
{
    public class ControlledTimeSource : IControlledTimeSource
    {
        private static readonly long TicksInSecond = Duration.FromSeconds(1).BclCompatibleTicks;

        private readonly FakeClock _internalClock;

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

        public Instant Now => _internalClock.GetCurrentInstant();

        public ControlledTimeSource(Instant initial)
        {
            _internalClock = new FakeClock(initial);
        }

        public void AddHours(int hours)
        {
            _internalClock.AdvanceHours(hours);
        }

        public void AddMinutes(long minutes)
        {
            _internalClock.AdvanceMinutes(minutes);
        }

        public void AddSeconds(long seconds)
        {
            _internalClock.AdvanceSeconds(seconds);
        }

        public void UpdateClock()
        {
            var delta = (long)(TicksInSecond * Speed);
            _internalClock.AdvanceTicks(delta);
        }

        public void Reset(Instant instant)
        {
            _internalClock.Reset(instant);
        }
    }
}
