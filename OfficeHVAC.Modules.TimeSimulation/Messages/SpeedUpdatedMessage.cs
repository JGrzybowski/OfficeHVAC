namespace OfficeHVAC.Modules.TimeSimulation.Messages
{
    public class SpeedUpdatedMessage
    {
        public double Speed { get; }

        public SpeedUpdatedMessage(double speed)
        {
            Speed = speed;
        }
    }
}