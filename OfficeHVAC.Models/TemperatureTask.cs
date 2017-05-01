using NodaTime;

namespace OfficeHVAC.Models
{
    public class TemperatureTask
    {
        public double InitialTemperature { get; }
        public double DesiredTemperature { get; }
        public Duration TimeLimit { get; }

        public TemperatureTask(double initialTemperature, double desiredTemperature, Duration timeLimit)
        {
            InitialTemperature = initialTemperature;
            DesiredTemperature = desiredTemperature;
            TimeLimit = timeLimit;
        }
    }
}
