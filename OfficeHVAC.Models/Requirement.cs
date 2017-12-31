using NodaTime;

namespace OfficeHVAC.Models {
    public class Requirement<TParam>
    {
        public Instant StartTime { get; }
        public Instant EndTime { get; }
        public TParam ExpextedParamValue { get; }
        
        public Requirement(Instant startTime, Instant endTime, TParam expextedParamValue)
        {
            StartTime = startTime;
            EndTime = endTime;
            ExpextedParamValue = expextedParamValue;
        }
    }
}