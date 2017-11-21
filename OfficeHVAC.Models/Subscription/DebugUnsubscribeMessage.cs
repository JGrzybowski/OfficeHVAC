using Akka.Actor;

namespace OfficeHVAC.Models.Subscription
{
    public class DebugUnsubscribeMessage
    {
        public IActorRef Subscriber { get; }

        public DebugUnsubscribeMessage(IActorRef subscriber)
        {
            Subscriber = subscriber;
        }
    }
}
