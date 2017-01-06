using System;

namespace OfficeHVAC.Models
{
    public class RealTimeSource : ITimeSource
    {
        public DateTime Now => DateTime.Now;
    }
}
