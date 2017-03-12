using OfficeHVAC.Models;

namespace OfficeHVAC.Messages
{
    public class TemperatureValueMessage : ParameterValueMessage
    {
        public TemperatureValueMessage(double value) : base(SensorType.Temperature, value) { }

        public double Temperature => (double)(Value);
    }
}
