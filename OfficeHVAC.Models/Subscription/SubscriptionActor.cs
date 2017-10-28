using Akka.Actor;

namespace OfficeHVAC.Models.Subscription
{
    public class SubscriptionActor : ReceiveActor
    {
        private Subscription subscription = new Subscription();

        public SubscriptionActor()
        {
            //Subscribtion handling
            Receive<SubscribeMessage>(message => OnSubscribeMessage(message));
            Receive<UnsubscribeMessage>(message => OnUnsubscribeMesssage(message));
            Receive<SendToSubscribersMessage>(message => SendSubscribtionNewsletter(message.Newsletter));
        }

        protected virtual void OnSubscribeMessage(SubscribeMessage message) =>
            subscription.AddSubscriber(message.Subscriber);
        
        protected virtual void OnUnsubscribeMesssage(UnsubscribeMessage message) =>
            subscription.RemoveSubscriber(message.Subscriber);

        protected virtual void SendSubscribtionNewsletter(object msg) => 
            subscription.SendToAllSubscribers(msg, Self);
    }
}