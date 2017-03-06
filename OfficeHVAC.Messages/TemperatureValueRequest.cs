using OfficeHVAC.Models;

namespace OfficeHVAC.Messages
{
    public class TemperatureValueRequest : ParameterValueRequest
    {
        public TemperatureValueRequest() : base(SensorTypes.Temperature) { }
    }
}
