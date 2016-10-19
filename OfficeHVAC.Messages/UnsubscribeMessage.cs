using Akka.Actor;

namespace OfficeHVAC.Messages
{
    public class UnsubscribeMessage
    {
        public IActorRef Subscriber { get; private set; }

        public UnsubscribeMessage(IActorRef subscriber)
        {
            this.Subscriber = subscriber;
        }
    }
}
