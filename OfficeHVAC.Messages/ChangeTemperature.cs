namespace OfficeHVAC.Messages
{
    public class ChangeTemperature
    {
        public float DeltaT { get; }

        public ChangeTemperature(float deltaT)
        {
            DeltaT = deltaT;
        }
    }
}
