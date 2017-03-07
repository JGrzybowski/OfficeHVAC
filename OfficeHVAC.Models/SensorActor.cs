using Akka.Actor;

namespace OfficeHVAC.Models
{
    public class SensorActor : ISensorActor
    {
        public string Id { get; set; }
        public SensorTypes Type { get; set; }
        public IActorRef Actor { get; }

        public SensorActor(string id, SensorTypes type, IActorRef actor)
        {
            Id = id;
            Type = type;
            Actor = actor;
        }
    }
}
