using Akka.Actor;

namespace OfficeHVAC.Factories.ActorPaths
{
    public interface IActorPathBuilder
    {
        ActorPath ActorPath();
    }
}
