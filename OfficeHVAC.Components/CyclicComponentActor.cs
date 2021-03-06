﻿using System.Collections.Generic;
using Akka.Actor;
using NodaTime;
using OfficeHVAC.Messages;
using OfficeHVAC.Models.Subscription;

namespace OfficeHVAC.Components
{
    public abstract class CyclicComponentActor<TInternalStatus, TParameter> : ComponentActor<TInternalStatus, TParameter> 
        where TInternalStatus : ComponentStatus<TParameter>
    {
        protected IActorRef SubsciptionManager { get; }

        protected Duration Threshold { get; set; } = Duration.FromSeconds(5);
        protected Duration ThresholdBuffer { get; set; } = Duration.Zero;

        public CyclicComponentActor(IEnumerable<string> subscriptionSources) : base(subscriptionSources)
        {
            SubsciptionManager = Context.ActorOf<SubscriptionActor>();
        }

        protected override void Uninitialized()
        {
            RegisterSubscribtionReceives();
            base.Uninitialized();
        }

        protected override void Initialized()
        {
            RegisterSubscribtionReceives();
            base.Initialized();
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
            OnTimeUpdated(timeDiff, now);
        }

        protected virtual void OnTimeUpdated(Duration timeDiff, Instant newTime) { }
        
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