using Akka.Actor;

namespace OfficeHVAC.Models
{
    public class SensorActorRef : ISensorActorRef
    {
        public string Id { get; set; }
        public SensorType Type { get; set; }
        public IActorRef Actor { get; }

        public SensorActorRef(string id, SensorType type, IActorRef actor)
        {
            Id = id;
            Type = type;
            Actor = actor;
        }

        public object Clone() => new SensorActorRef(Id, Type, Actor);
    }
}
