using Akka.Actor;

namespace OfficeHVAC.Messages
{
    public class RoomAvaliabilityMessage
    {
        public readonly IActorRef RoomActor;

        public RoomAvaliabilityMessage(IActorRef roomActorRef)
        {
            RoomActor = roomActorRef;
        }
    }
}
