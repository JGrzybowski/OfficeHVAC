namespace OfficeHVAC.Messages {
    public class SetParameterValueMessage<T> {
        public T Value { get; }
        
        public SetParameterValueMessage(T value)
        {
            Value = value;
        }
    }
}