using System;
using OfficeHVAC.Models;

namespace OfficeHVAC.Simulators.Tests
{
    public class TimeSourceFake : ITimeSource
    {
        public DateTime Now { get; set; }

        public TimeSourceFake(DateTime dateTime)
        {
            Now = dateTime;
        }
    }
}
