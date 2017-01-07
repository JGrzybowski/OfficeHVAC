using NodaTime;
using NodaTime.Testing;
using System;

namespace OfficeHVAC.Models
{
    public class ControlledTimeSource : ITimeSource
    {
        private FakeClock _internalClock;

        public double Speed { get; set; }

        public Instant Now { get {throw new NotImplementedException();} }
        
        public ControlledTimeSource(Instant initial)
        {
            this._internalClock = new FakeClock(initial);
        }

        public void AddHours(long hours)
        {
            throw new NotImplementedException();
        }

        public void AddMinutes(long minutes)
        {
            throw new NotImplementedException();
        }
        public void AddSeconds(long seconds)
        {
            throw new NotImplementedException();
        }

        public void UpdateClock() { }

        public void Reset(Instant instant) { }
    }
}
