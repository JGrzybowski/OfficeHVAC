using NodaTime;
using System;

namespace OfficeHVAC.Models
{
    public class Requirements
    {
        public string Id = Guid.NewGuid().ToString();

        public Instant Deadline;

        public ParameterValuesCollection Parameters { get; }

        public Requirements(Instant deadline, ParameterValuesCollection parameters)
        {
            Deadline = deadline;
            Parameters = parameters;
        }
    }
}
