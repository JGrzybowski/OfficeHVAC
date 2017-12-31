namespace OfficeHVAC.Modules.TimeSimulation.Messages
{
    public class SetSpeedMessage
    {
        public SetSpeedMessage(double speed)
        {
            Speed = speed;
        }
        public double Speed { get;  }
    }
}