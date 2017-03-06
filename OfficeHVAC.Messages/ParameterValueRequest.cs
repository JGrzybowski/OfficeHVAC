using OfficeHVAC.Models;

namespace OfficeHVAC.Messages
{
    public class ParameterValueRequest
    {
        public SensorTypes ParamType { get; }

        public ParameterValueRequest(SensorTypes paramType)
        {
            ParamType = paramType;
        }
    }
}
