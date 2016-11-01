using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Factories.Propses;

namespace OfficeHVAC.Modules.RoomSimulator.Factories
{
    public interface IRoomSimulatorActorPropsFactory : IPropsFactory
    {
        float Temperature { get; set; }

        string RoomName { get; set; }

        IActorPathBuilder CompanyPathBuilder { get; }
    }
}