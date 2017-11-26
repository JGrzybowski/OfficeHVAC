using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using NodaTime;
using OfficeHVAC.Models.Actors;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TimeSimulation.Messages;


namespace OfficeHVAC.Components
{
    public abstract class ComponentActor<TInternalStatus, TParameter> : DebuggableActor<TInternalStatus>
        where TInternalStatus : ComponentStatus<TParameter>
    {
        protected string Id { get; set; }
        
        protected TParameter ParameterValue { get; set; }
        
        protected Instant Timestamp { get; set; } = Instant.MinValue;
        private bool TimeStampInitialized => Timestamp != Instant.MinValue;

        protected virtual bool ReceivedInitialData() => TimeStampInitialized;

        protected List<ICanTell> SubscriptionSources;
        
        public ComponentActor(IEnumerable<string> subscribtionsSources, TInternalStatus initialStatus)
        {
            SetInternalStatus(initialStatus);
            
            SubscriptionSources = new List<ICanTell>(
                subscribtionsSources.Select(path => Context.ActorSelection(path)));
            
            Become(Initialized);
        }
        
        public ComponentActor(IEnumerable<string> subscribtionsSources)
        {
            SubscriptionSources = new List<ICanTell>(
                subscribtionsSources.Select(path => Context.ActorSelection(path)));
            Become(Uninitialized);
        }

        protected virtual void Initialized()
        {
            Receive<TimeChangedMessage>(
                msg => OnTimeChangedMessage(msg), 
                msg => msg.Now > Timestamp);
            
            RegisterDebugReceives();
        }

        protected virtual void Uninitialized()
        {
            Receive<TimeChangedMessage>(
                msg =>
                {
                    Timestamp = msg.Now;
                    if(ReceivedInitialData()) Become(Initialized);
                }, 
                msg => msg.Now > Timestamp);
            
            RegisterDebugReceives();
        }
        
        protected override void SetInternalStatus(TInternalStatus msg)
        {
            Id = msg.Id;
            Timestamp = msg.TimeStamp;
            ParameterValue = msg.ParameterValue;
            base.SetInternalStatus(msg);
        }

        protected virtual void OnTimeChangedMessage(TimeChangedMessage msg)
        {
            Timestamp = msg.Now;
            InformAboutInternalState();
        }

        protected override void PreStart()
        {
            foreach (var subscriptionSource in SubscriptionSources)
                subscriptionSource.Tell(new SubscribeMessage(Self), Self);
        }
    }
    
    public class ComponentStatus<T>
    {
        public string Id { get; }
        public T ParameterValue { get; }
        public Instant TimeStamp { get; }

        public ComponentStatus(string id, T parameterValue, Instant timeStamp)
        {
            Id = id;
            ParameterValue = parameterValue;
            TimeStamp = timeStamp;
        }
    }
}