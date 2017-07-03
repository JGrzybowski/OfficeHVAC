using Akka.Actor;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;

namespace OfficeHVAC.Modules.TimeSimulation
{
    public class TimeSimulatorBridgeActor : ReceiveActor
    {
        private ITimeControlViewModel ViewModel { get; }
        private IActorRef TimeSimulatorRef { get; }

        public TimeSimulatorBridgeActor(ITimeControlViewModel viewModel, IControlledTimeSource controlledTimeSource)
        {
            ViewModel = viewModel;

            var simulatorProps = Props.Create(()=>new TimeSimulatorActor(controlledTimeSource));
            TimeSimulatorRef = Context.ActorOf(simulatorProps);

            Receive<TickClockMessage>(msg => TimeSimulatorRef.Forward(msg));
            Receive<AddMinutesMessage>(msg => TimeSimulatorRef.Forward(msg));
            Receive<SetSpeedMessage>(msg => TimeSimulatorRef.Forward(msg));

            Receive<TimeChangedMessage>(msg => ViewModel.TimeText = msg.Now.ToString("hh:MM:ss", null));

            TimeSimulatorRef.Tell(new SubscriptionMessage(), Self);
        }
    }
}
