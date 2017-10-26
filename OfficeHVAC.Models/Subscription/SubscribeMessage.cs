using Akka.Actor;

namespace OfficeHVAC.Models.Subscription
{
    public class SubscribeMessage
    {
        public IActorRef Subscriber { get; private set; }

        public SubscribeMessage(IActorRef subscriber)
        {
            Subscriber = subscriber;
        }
    }
}
