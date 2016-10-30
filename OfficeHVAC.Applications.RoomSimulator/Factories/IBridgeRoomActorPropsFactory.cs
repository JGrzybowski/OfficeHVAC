using OfficeHVAC.Applications.RoomSimulator.ViewModels;
using OfficeHVAC.Factories.Propses;

namespace OfficeHVAC.Applications.RoomSimulator.Factories
{
    public interface IBridgeRoomActorPropsFactory : IPropsFactory
    {
        IRoomSimulatorActorPropsFactory RoomSimulatorActorPropsFactory { get; }
        IRoomViewModel ViewModel { get; }
    }
}
