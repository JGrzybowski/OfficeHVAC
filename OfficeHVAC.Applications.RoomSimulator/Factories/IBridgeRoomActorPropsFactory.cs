using OfficeHVAC.Factories.Propses;

namespace OfficeHVAC.Applications.RoomSimulator.Factories
{
    public interface IBridgeRoomActorPropsFactory : IPropsFactory
    {
        IRoomSimulatorActorPropsFactory RoomSimulatorActorPropsFactory { get; }
        ViewModels.RoomViewModel ViewModel { get; set; }
    }
}