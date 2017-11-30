using Akka.Actor;
using OfficeHVAC.Models.Subscription;

namespace OfficeHVAC.Models.Actors
{
    public abstract class DebuggableActor<TInternalStatus> : ReceiveActor
    {
        protected IActorRef DebugSubscriptionManager;

        public DebuggableActor()
        {
            DebugSubscriptionManager = Context.ActorOf<SubscriptionActor>();
            RegisterDebugReceives();
        }

        protected void RegisterDebugReceives()
        {
            Receive<SetInternalStatusMessage<TInternalStatus>>(msg => SetInternalStatus(msg.Status));
            Receive<DebugSubscribeMessage>(msg =>
            {
                DebugSubscriptionManager.Tell(new SubscribeMessage(msg.Subscriber));
                msg.Subscriber.Tell(GenerateInternalState());
            });
            Receive<DebugUnsubscribeMessage>(msg => DebugSubscriptionManager.Tell(new UnsubscribeMessage(msg.Subscriber)));
        }

        protected virtual void SetInternalStatus(TInternalStatus msg) => 
            InformAboutInternalState();
        
        protected abstract TInternalStatus GenerateInternalState();
        
        protected void InformDebugSubscribers(TInternalStatus internalStatus)
        {
            DebugSubscriptionManager.Tell(new SendToSubscribersMessage(internalStatus));
        }
        protected void InformAboutInternalState() => InformDebugSubscribers(GenerateInternalState());
    }
}