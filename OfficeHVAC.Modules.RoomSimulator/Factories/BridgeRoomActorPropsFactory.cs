using Akka.Actor;
using OfficeHVAC.Modules.RoomSimulator.Actors;
using OfficeHVAC.Modules.RoomSimulator.ViewModels;

namespace OfficeHVAC.Modules.RoomSimulator.Factories
{
    public class BridgeRoomActorPropsFactory: IBridgeRoomActorPropsFactory
    {
        public IRoomSimulatorActorPropsFactory RoomSimulatorActorPropsFactory { get; }
        public IRoomViewModel ViewModel { get; set; }

        public BridgeRoomActorPropsFactory(IRoomSimulatorActorPropsFactory roomSimulatorActorPropsFactory)
        {
            RoomSimulatorActorPropsFactory = roomSimulatorActorPropsFactory;
        }

        public Props Props()
        {
            return Akka.Actor.Props.Create(() => new RoomBridgeActor(ViewModel, RoomSimulatorActorPropsFactory.Props()));
        }
    }
}
