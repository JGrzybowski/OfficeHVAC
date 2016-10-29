using Akka.Actor;
using OfficeHVAC.Actors;
using OfficeHVAC.Applications.RoomSimulator.ViewModels;

namespace OfficeHVAC.Applications.RoomSimulator.Actors
{
    public class RoomBridgeActor : BridgeActor<RoomViewModel>
    {
        private readonly Props _roomActorProps;
        private IActorRef roomActorRef;
        
        public RoomBridgeActor(RoomViewModel viewModel, Props roomActorProps) : base(viewModel)
        {
            _roomActorProps = roomActorProps;
        }


    }
}
