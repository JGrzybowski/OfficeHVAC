using Akka.Actor;

namespace OfficeHVAC.Models
{
    public interface ISensorActorRef : ISensor
    {
        ParameterValue ParamValue { get; }
            
        IActorRef Actor { get; }
    }
}