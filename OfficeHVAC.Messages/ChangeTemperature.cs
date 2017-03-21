namespace OfficeHVAC.Messages
{
    public class ChangeTemperature
    {
        public double DeltaT { get; }

        public ChangeTemperature(double deltaT)
        {
            DeltaT = deltaT;
        }
    }
}
