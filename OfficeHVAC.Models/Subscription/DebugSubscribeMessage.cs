using Akka.Actor;

namespace OfficeHVAC.Models.Subscription
{
    public class DebugSubscribeMessage
    {
        public IActorRef Subscriber { get; }

        public DebugSubscribeMessage(IActorRef subscriber)
        {
            Subscriber = subscriber;
        }
    }
}
