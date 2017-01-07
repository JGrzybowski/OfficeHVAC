using NodaTime;
using System;

namespace OfficeHVAC.Models
{
    public class ControlledTimeSource : ITimeSource
    {
        public Instant Now { get {throw new NotImplementedException();} }
    }
}
