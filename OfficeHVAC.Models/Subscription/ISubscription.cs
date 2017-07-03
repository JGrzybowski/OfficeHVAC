using Akka.Actor;

namespace OfficeHVAC.Models.Subscription
{
    public interface ISubscription
    {
        void AddSubscriber(IActorRef subscriber);
        void RemoveSubscriber(IActorRef subscriber);
        void SendToAllSubscribers<T>(T message, IActorRef sender = null);
    }
}