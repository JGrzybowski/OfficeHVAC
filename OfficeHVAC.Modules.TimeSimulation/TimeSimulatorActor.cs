﻿using Akka.Actor;
using NodaTime;
using OfficeHVAC.Messages;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;

namespace OfficeHVAC.Modules.TimeSimulation
{
    public class TimeSimulatorActor : ReceiveActor
    {
        private IControlledTimeSource TimeSource { get; }

        private readonly Subscription timeUpdateSubscription = new Subscription();

        public TimeSimulatorActor(IControlledTimeSource timeSource)
        {
            TimeSource = timeSource;

            Receive<SubscribeMessage>(msg =>
            {
                timeUpdateSubscription.AddSubscriber(msg.Subscriber);
                msg.Subscriber.Tell(new TimeChangedMessage(TimeSource.Now));
            });
            Receive<UnsubscribeMessage>(msg => timeUpdateSubscription.RemoveSubscriber(msg.Subscriber));

            Receive<TickClockMessage>(msg =>
            {
                var t = TimeSource.Now;
                TimeSource.UpdateClock();
                NotifyAboutTimeChange(t);
            });

            Receive<AddMinutesMessage>(msg =>
            {
                var t = TimeSource.Now;
                TimeSource.AddMinutes(msg.Minutes);
                NotifyAboutTimeChange(t);
            });
            
            Receive<SetSpeedMessage>(msg =>
            {
                TimeSource.Speed = msg.Speed;
                Sender.Tell(new SpeedUpdatedMessage(TimeSource.Speed));
            });
        }

        public void NotifyAboutTimeChange(Instant oldInstant)
        {
            var timeMsg = new TimeChangedMessage(TimeSource.Now);
            timeUpdateSubscription.SendToAllSubscribers(timeMsg, Self);
        }

        public static Props Props(IControlledTimeSource timeSource) =>
            Akka.Actor.Props.Create(() => new TimeSimulatorActor(timeSource));
    }
}
