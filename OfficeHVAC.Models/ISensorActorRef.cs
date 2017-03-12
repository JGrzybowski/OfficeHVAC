using Akka.Actor;

namespace OfficeHVAC.Models
{
    public interface ISensorActorRef : ISensor
    {        
        IActorRef Actor { get; }
    }
}