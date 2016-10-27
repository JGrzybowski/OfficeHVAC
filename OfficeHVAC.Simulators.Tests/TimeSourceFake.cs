using System;
using OfficeHVAC.Models;

namespace OfficeHVAC.Simulators.Tests
{
    public class TimeSourceFake : ITimeSource
    {
        public TimeSourceFake(DateTime dateTime)
        {
            this.Now = dateTime;
        }
        public DateTime Now { get; set; }
    }
}
