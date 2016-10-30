using Akka.Actor;
using OfficeHVAC.Applications.RoomSimulator.Actors;
using OfficeHVAC.Applications.RoomSimulator.ViewModels;

namespace OfficeHVAC.Applications.RoomSimulator.Factories
{
    public class BridgeRoomActorPropsFactory: IBridgeRoomActorPropsFactory
    {
        public IRoomSimulatorActorPropsFactory RoomSimulatorActorPropsFactory { get; }
        public IRoomViewModel ViewModel { get; }

        public BridgeRoomActorPropsFactory(IRoomSimulatorActorPropsFactory roomSimulatorActorPropsFactory, IRoomViewModel viewModel)
        {
            RoomSimulatorActorPropsFactory = roomSimulatorActorPropsFactory;
            ViewModel = viewModel;
        }

        public Props Props()
        {
            return Akka.Actor.Props.Create(() => new RoomBridgeActor(ViewModel, RoomSimulatorActorPropsFactory.Props()));
        }
    }
}
