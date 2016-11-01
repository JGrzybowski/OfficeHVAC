using Akka.Actor;
using OfficeHVAC.Applications.RoomSimulator.Actors;
using OfficeHVAC.Applications.RoomSimulator.ViewModels;

namespace OfficeHVAC.Applications.RoomSimulator.Factories
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
