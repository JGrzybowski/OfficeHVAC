namespace OfficeHVAC.Messages
{
    public class SetTemperature
    {
        public float Temperature { get; }

        public SetTemperature(float temperature)
        {
            Temperature = temperature;
        }
    }
}