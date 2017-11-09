using Akka.Actor;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;

namespace OfficeHVAC.Applications.BuildingSimulator.Actors
{
    public class TemperatureModelActor : ReceiveActor
    {
        private ITemperatureModel model;
        
        private IActorRef subscriptionManager;
        
        public TemperatureModelActor()
        {
            subscriptionManager = Context.ActorOf<SubscriptionActor>();
            
            Receive<SubscribeMessage>(msg =>
            {
                subscriptionManager.Tell(msg);
                msg.Subscriber.Tell(model);
            });
            
            Receive<UnsubscribeMessage>(msg => subscriptionManager.Tell(msg));
            Receive<ITemperatureModel>(msg =>
            {
                model = msg;
                subscriptionManager.Tell(new SendToSubscribersMessage(model));
            });
        }
    }
}