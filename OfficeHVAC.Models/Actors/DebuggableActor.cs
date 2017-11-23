using Akka.Actor;
using OfficeHVAC.Models.Subscription;

namespace OfficeHVAC.Models.Actors
{
    public abstract class DebuggableActor<T> : ReceiveActor
    {
        protected IActorRef DebugSubscriptionManager;

        public DebuggableActor()
        {
            DebugSubscriptionManager = Context.ActorOf<SubscriptionActor>();
            RegisterDebugSubscribers();
        }

        protected void RegisterDebugSubscribers()
        {
            Receive<DebugSubscribeMessage>(msg =>
            {
                DebugSubscriptionManager.Tell(new SubscribeMessage(msg.Subscriber));
                msg.Subscriber.Tell(GenerateInternalState());
            });
            Receive<DebugUnsubscribeMessage>(msg => DebugSubscriptionManager.Tell(new UnsubscribeMessage(msg.Subscriber)));
        }

        protected abstract T GenerateInternalState();
        protected void InformDebugSubscribers(T internalStatus)
        {
            DebugSubscriptionManager.Tell(new SendToSubscribersMessage(internalStatus));
        }
        protected void InformAboutInternalState() => InformDebugSubscribers(GenerateInternalState());
    }
}