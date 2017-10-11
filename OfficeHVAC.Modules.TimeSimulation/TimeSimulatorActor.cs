using Akka.Actor;
using NodaTime;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;

namespace OfficeHVAC.Modules.TimeSimulation
{
    public class TimeSimulatorActor : ReceiveActor
    {
        private IControlledTimeSource TimeSource { get; }

        private readonly Subscription TimeUpdateSubscription = new Subscription();

        public TimeSimulatorActor(IControlledTimeSource timeSource)
        {
            TimeSource = timeSource;

            Receive<SubscriptionMessage>(msg => this.TimeUpdateSubscription.AddSubscriber(Sender));
            Receive<UnsubscriptionMessage>(msg => this.TimeUpdateSubscription.RemoveSubscriber(Sender));

            Receive<TickClockMessage>(msg =>
            {
                var t = TimeSource.Now;
                this.TimeSource.UpdateClock();
                NotifyAboutTimeChange(t);
            });

            Receive<AddMinutesMessage>(msg =>
            {
                var t = TimeSource.Now;
                this.TimeSource.AddMinutes(msg.Minutes);
                NotifyAboutTimeChange(t);
            });
            
            Receive<SetSpeedMessage>(msg => this.TimeSource.Speed = msg.Speed);
        }

        public void NotifyAboutTimeChange(Instant oldInstant)
        {
            var timeDelta = TimeSource.Now - oldInstant;
            var timeMsg = new TimeChangedMessage(timeDelta, TimeSource.Now);
            this.TimeUpdateSubscription.SendToAllSubscribers(timeMsg, Self);
        }

        public static Props Props(IControlledTimeSource timeSource) =>
            Akka.Actor.Props.Create(() => new TimeSimulatorActor(timeSource));
    }
}
