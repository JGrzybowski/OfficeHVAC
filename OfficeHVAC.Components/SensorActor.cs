using Akka.Actor;
using NodaTime;
using OfficeHVAC.Models.Subscription;

namespace OfficeHVAC.Components
{
    public abstract class SensorActor<TInternalStatus, TParameter> : ComponentActor<TInternalStatus, TParameter> 
        where TInternalStatus : ComponentStatus<TParameter>
    {
        private IActorRef SubsciptionManager { get; }
        
        public SensorActor(string timeSourceActorPath) : base(new []{timeSourceActorPath})
        {
            SubsciptionManager = Context.ActorOf<SubscriptionActor>();
            Receive<SubscribeMessage>(msg => SubsciptionManager.Tell(msg));
            Receive<UnsubscribeMessage>(msg => SubsciptionManager.Tell(msg));
        }    

        public class Status : ComponentStatus<TParameter>
        {
            public Status(string id, TParameter parameterValue, Instant timeStamp) 
                : base(id, parameterValue, timeStamp) 
            { }
        }
    }

    public abstract class SensorSimulatorActor<TInternalStatus, TParameter> : SensorActor<TInternalStatus, TParameter> 
        where TInternalStatus : ComponentStatus<TParameter> 
    {
        protected SensorSimulatorActor(string timeSourceActorPath) : base(timeSourceActorPath)
        {
            
        }
    }
}