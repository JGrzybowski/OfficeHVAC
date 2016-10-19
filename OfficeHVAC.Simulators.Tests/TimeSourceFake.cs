using System;
using OfficeHVAC.Models;

namespace OfficeHVAC.Simulators.Tests
{
    public class TimeSourceFake : ITimeSource
    {
        public TimeSourceFake(DateTime dateTime)
        {
            this.Time = dateTime;
        }
        public DateTime Time { get; set; }
    }
}
