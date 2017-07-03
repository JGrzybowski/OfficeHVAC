namespace OfficeHVAC.Modules.TimeSimulation.Messages
{
    internal class SetSpeedMessage
    {
        public SetSpeedMessage(double speed)
        {
            Speed = speed;
        }
        public double Speed { get;  }
    }
}