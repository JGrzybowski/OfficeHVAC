namespace OfficeHVAC.Models.Subscription
{
    public class SendToSubscribersMessage
    {
        public object Newsletter { get; }

        public SendToSubscribersMessage(object newsletter)
        {
            Newsletter = newsletter;
        }
    }
}