using OfficeHVAC.Factories.Propses;
using OfficeHVAC.Modules.RoomSimulator.ViewModels;

namespace OfficeHVAC.Modules.RoomSimulator.Factories
{
    public interface IBridgeRoomActorPropsFactory : IPropsFactory
    {
        IRoomSimulatorActorPropsFactory RoomSimulatorActorPropsFactory { get; }
        IRoomViewModel ViewModel { get; set; }
    }
}
