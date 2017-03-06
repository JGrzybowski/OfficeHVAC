using OfficeHVAC.Models;

namespace OfficeHVAC.Messages
{
    public class TemperatureValueMessage : ParameterValueMessage
    {
        public TemperatureValueMessage(double value) : base(SensorTypes.Temperature, value) { }

        public double Temperature => (double)(Value);
    }
}
