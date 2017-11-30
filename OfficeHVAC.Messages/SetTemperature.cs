namespace OfficeHVAC.Messages
{
    public class SetTemperature
    {
        public double Temperature { get; }

        public SetTemperature(double temperature)
        {
            Temperature = temperature;
        }
    }
}
