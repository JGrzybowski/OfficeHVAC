using Akka.Actor;
using System.Collections.Generic;

namespace OfficeHVAC.Models.Subscription
{
    public class Subscription : ISubscription
    {
        private readonly HashSet<IActorRef> subscribers = new HashSet<IActorRef>();

        public void AddSubscriber(IActorRef subscriber) => subscribers.Add(subscriber);

        public void RemoveSubscriber(IActorRef subscriber) => subscribers.Remove(subscriber);

        public void SendToAllSubscribers<T>(T message, IActorRef sender = null)
        {
            foreach(var subscriber in subscribers)
                subscriber.Tell(message, sender);
        }
    }
}
