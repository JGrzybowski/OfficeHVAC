using System;

namespace OfficeHVAC.Models
{
    public class ParameterValue : IToMessage<IParameterValueMessage>, ICloneable
    {
        public SensorType ParameterType { get; set; }

        public ICloneable Value { get; set; }

        public IParameterValueMessage ToMessage() => this.Clone() as IParameterValueMessage;

        public object Clone() => new ParameterValue()
        {
            ParameterType = ParameterType,
            Value = Value.Clone() as ICloneable
        };
    }

    public interface IParameterValueMessage : ICloneable
    {
        SensorType ParameterType { get; }
        object Value { get; }
    }
}