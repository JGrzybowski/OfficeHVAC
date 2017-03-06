using OfficeHVAC.Models;

namespace OfficeHVAC.Messages
{
    public class ParameterValueMessage
    {
        public SensorTypes ParamType { get; }

        public object Value { get; }

        public ParameterValueMessage(SensorTypes paramType, object value)
        {
            ParamType = paramType;
            Value = value;
        }
    }
}
