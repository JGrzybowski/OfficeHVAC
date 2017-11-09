using System;

namespace OfficeHVAC.Models
{
    public class ParameterValue : IToMessage<IParameterValueMessage>, IParameterValueMessage
    {
        public ParameterValue() { }

        public ParameterValue(SensorType parameterType, object value)
        {
            ParameterType = parameterType;
            Value = value;
        }

        public SensorType ParameterType { get; set; }

        public object Value { get; set; }

        public IParameterValueMessage ToMessage() => Clone() as IParameterValueMessage;

        public object Clone()
        {
            var clone = new ParameterValue()
            {
                ParameterType = ParameterType,
            };
            clone.Value = Value.GetType().IsValueType ? Value : (Value as ICloneable).Clone();

            return clone;
        }

        public class Request
        {
            public Request(SensorType parameterType) { ParameterType = parameterType; }

            public SensorType ParameterType { get; set; }
        }
    }

    public interface IParameterValueMessage : ICloneable
    {
        SensorType ParameterType { get; }
        object Value { get; }
    }
}