using Akka.Actor;

namespace OfficeHVAC.Messages
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
