using Akka.TestKit;
using NodaTime;
using NodaTime.Testing;
using System;

namespace OfficeHVAC.Modules.TimeSimulation.TimeSources
{
    public class ControlledTimeSource : IControlledTimeSource
    {
        private static readonly long TicksInSecond = Duration.FromSeconds(1).BclCompatibleTicks;
        
        private readonly FakeClock internalClock;
        private readonly TestScheduler scheduler;

        private double speed = 1;
        public double Speed
        {
            get => speed;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Speed cannot have negative value");
                speed = value;
            }
        }

        public Instant Now => internalClock.GetCurrentInstant();

        public ControlledTimeSource(Instant initial, TestScheduler scheduler)
        {
            internalClock = new FakeClock(initial);
            this.scheduler = scheduler;
            Reset(initial);
        }

        public void AddHours(int hours)      => AdvanceClock(Duration.FromHours(hours));
        public void AddMinutes(long minutes) => AdvanceClock(Duration.FromMinutes(minutes));
        public void AddSeconds(long seconds) => AdvanceClock(Duration.FromSeconds(seconds));

        public void UpdateClock()
        {
            var delta = (long)(TicksInSecond * Speed);
            AdvanceClock(Duration.FromTicks(delta));
        }

        private void AdvanceClock(Duration duration)
        {
            internalClock.Advance(duration);
            scheduler?.AdvanceTo(Now.ToDateTimeOffset());
        } 

        public void Reset(Instant instant)
        {
            internalClock.Reset(instant);
            scheduler?.AdvanceTo(Now.ToDateTimeOffset());
        }
    }
}
