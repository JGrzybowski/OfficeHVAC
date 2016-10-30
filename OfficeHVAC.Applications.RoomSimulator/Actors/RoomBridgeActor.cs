using Akka.Actor;
using OfficeHVAC.Actors;
using OfficeHVAC.Applications.RoomSimulator.ViewModels;
using OfficeHVAC.Messages;

namespace OfficeHVAC.Applications.RoomSimulator.Actors
{
    public class RoomBridgeActor : BridgeActor<IRoomViewModel>
    {
        public const string RoomActorName = "room";

        private readonly Props _roomActorProps;
        private IActorRef _roomActorRef;
        
        public RoomBridgeActor(IRoomViewModel viewModel, Props roomActorProps) : base(viewModel)
        {
            _roomActorProps = roomActorProps;
            _roomActorRef = Context.ActorOf(_roomActorProps, RoomActorName);
            
            Receive<SetTemperature>(msg => _roomActorRef.Tell(msg));
            Receive<ChangeTemperature>(msg => _roomActorRef.Tell(msg));
            Receive<RoomStatusMessage>(msg => ViewModel.Temperature = msg.Temperature);
        }
    }
}
