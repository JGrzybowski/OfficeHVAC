using System;
using System.Collections.Generic;
using Akka.Actor;
using NodaTime;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TimeSimulation.Messages;

namespace OfficeHVAC.Components
{
    public abstract class SensorActor<TInternalStatus, TParameter> : ComponentActor<TInternalStatus, TParameter> 
        where TInternalStatus : ComponentStatus<TParameter>
    {
        protected IActorRef SubsciptionManager { get; }

        protected Duration Threshold { get; set; } = Duration.FromSeconds(5);
        protected Duration ThresholdBuffer { get; set; } = Duration.Zero;

        public SensorActor(IEnumerable<string> subscriptionSources) : base(subscriptionSources)
        {
            SubsciptionManager = Context.ActorOf<SubscriptionActor>();
        }
        public SensorActor(string timeSourceActorPath) : this(new[] {timeSourceActorPath}) { }

        protected override void Uninitialized()
        {
            base.Uninitialized();
            RegisterSubscribtionReceives();
        }

        protected override void Initialized()
        {
            base.Initialized();
            RegisterSubscribtionReceives();
        }

        protected override void OnTimeChangedMessage(TimeChangedMessage msg)
        {
            UpdateTime(msg.Now);
            if (ThresholdBuffer >= Threshold)
            {
                OnThresholdCrossed();
                ThresholdBuffer = Duration.Zero;
            }
        }
        
        protected override void UpdateTime(Instant now)
        {
            var timeDiff = now - Timestamp;
            Timestamp = now;
            ThresholdBuffer += timeDiff;
            OnTimeUpdated(timeDiff);
        }

        protected virtual void OnTimeUpdated(Duration timeDiff) { }
        
        protected void RegisterSubscribtionReceives()
        {
            Receive<SubscribeMessage>(msg => SubsciptionManager.Tell(msg));
            Receive<UnsubscribeMessage>(msg => SubsciptionManager.Tell(msg));
        }

        protected virtual void OnThresholdCrossed() { }
        
        public class Status : ComponentStatus<TParameter>
        {
            public Status(string id, TParameter parameterValue, Instant timestamp) 
                : base(id, parameterValue, timestamp) 
            { }
        }
    }
}