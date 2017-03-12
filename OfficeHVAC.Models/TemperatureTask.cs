using NodaTime;

namespace OfficeHVAC.Models
{
    public class TemperatureTask
    {
        public double Temperature { get; }
        public Instant Deadline { get; }

        public TemperatureTask(double temperature, Instant deadline)
        {
            Temperature = temperature;
            Deadline = deadline;
        }
    }
}
