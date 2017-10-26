using Akka.Actor;

namespace OfficeHVAC.Models.Subscription
{
    public class UnsubscribeMessage
    {
        public IActorRef Subscriber { get; private set; }

        public UnsubscribeMessage(IActorRef subscriber)
        {
            Subscriber = subscriber;
        }
    }
}
