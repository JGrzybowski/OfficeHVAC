using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;

namespace OfficeHVAC.Modules.TimeSimulation
{
    public class TimeSimulatorBridgeActor : BridgeActor<ITimeControlViewModel>
    {
        public TimeSimulatorBridgeActor(ITimeControlViewModel viewModel, IActorRef timeSimulatorActorRef) 
            : base(viewModel, timeSimulatorActorRef)
        {
            Receive<TickClockMessage>(msg => Actor.Forward(msg));
            Receive<AddMinutesMessage>(msg => Actor.Forward(msg));
            Receive<TimeChangedMessage>(msg => ViewModel.Time = msg.Now);

            Receive<SetSpeedMessage>(msg => Actor.Tell(msg));
            Receive<SpeedUpdatedMessage>(msg => ViewModel.SetupSpeed(msg.Speed));
            
            Actor.Tell(new SubscribeMessage(Self));
        }

        public TimeSimulatorBridgeActor(ITimeControlViewModel viewModel, IControlledTimeSource controlledTimeSource)
            : this(viewModel, Context.ActorOf(TimeSimulatorActor.Props(controlledTimeSource))) { }

        public static Props Props(ITimeControlViewModel viewModel, IControlledTimeSource controlledTimeSource) =>
            Akka.Actor.Props.Create(() => new TimeSimulatorBridgeActor(viewModel, controlledTimeSource));

        public static Props Props(ITimeControlViewModel viewModel, IActorRef timeSimulatorActorRef) =>
            Akka.Actor.Props.Create(() => new TimeSimulatorBridgeActor(viewModel, timeSimulatorActorRef));
    }
}
