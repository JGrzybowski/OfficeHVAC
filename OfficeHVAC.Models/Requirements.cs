using NodaTime;

namespace OfficeHVAC.Models
{
    public class Requirements
    {
        public Instant Deadline;

        public ParameterValuesCollection Parameters { get; }

        public Requirements(Instant deadline, ParameterValuesCollection parameters)
        {
            Deadline = deadline;
            Parameters = parameters;
        }
    }
}
