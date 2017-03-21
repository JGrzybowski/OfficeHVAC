using OfficeHVAC.Models;

namespace OfficeHVAC.Messages
{
    public class ParameterValueMessage
    {
        public SensorType ParamType { get; }

        public object Value { get; }

        public ParameterValueMessage(SensorType paramType, object value)
        {
            ParamType = paramType;
            Value = value;
        }
    }
}
