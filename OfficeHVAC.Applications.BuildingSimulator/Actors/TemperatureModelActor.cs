using Akka.Actor;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TemperatureSimulation;

namespace OfficeHVAC.Applications.BuildingSimulator.Actors
{
    public class TemperatureModelActor : ReceiveActor
    {
        private ITemperatureModel model;
        
        private IActorRef SubscriptionManager;
        
        public TemperatureModelActor()
        {
            SubscriptionManager = Context.ActorOf<SubscriptionActor>();
            
            Receive<SubscribeMessage>(msg =>
            {
                SubscriptionManager.Tell(msg);
                msg.Subscriber.Tell(model);
            });
            
            Receive<UnsubscribeMessage>(msg => SubscriptionManager.Tell(msg));
            Receive<ITemperatureModel>(msg =>
            {
                model = msg;
                SubscriptionManager.Tell(new SendToSubscribersMessage(model));
            });
        }
    }
}