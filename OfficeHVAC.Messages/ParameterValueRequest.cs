using OfficeHVAC.Models;

namespace OfficeHVAC.Messages
{
    public class ParameterValueRequest
    {
        public SensorType ParamType { get; }

        public ParameterValueRequest(SensorType paramType)
        {
            ParamType = paramType;
        }
    }
}
