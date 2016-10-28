using Akka.Actor;
using Akka.Configuration;

namespace OfficeHVAC.Actors
{
    public interface IConnectionConfig
    {
        ActorPath CompanyActorPath { get; }
    }
}
