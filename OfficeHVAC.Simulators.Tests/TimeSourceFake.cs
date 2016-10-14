using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeHVAC.Models;

namespace OfficeHVAC.Simulators.Tests
{
    public class TimeSourceFake : ITimeSource
    {
        public DateTime Time { get; set; }
    }
}
