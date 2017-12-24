namespace OfficeHVAC.Modules.TimeSimulation.Messages
{
    public class AddMinutesMessage
    {
        public AddMinutesMessage(long minutes)
        {
            Minutes = minutes;
        }
        public long Minutes { get; }
    }
}