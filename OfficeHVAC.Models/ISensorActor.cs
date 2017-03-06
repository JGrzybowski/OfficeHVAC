using Akka.Actor;

namespace OfficeHVAC.Models
{
    public interface ISensorActor : ISensor
    {
        IActorRef Actor { get; }
    }
}