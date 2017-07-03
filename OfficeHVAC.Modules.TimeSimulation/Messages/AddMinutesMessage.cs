namespace OfficeHVAC.Modules.TimeSimulation.Messages
{
    internal class AddMinutesMessage
    {
        public AddMinutesMessage(long minutes)
        {
            Minutes = minutes;
        }
        public long Minutes { get; }
    }
}