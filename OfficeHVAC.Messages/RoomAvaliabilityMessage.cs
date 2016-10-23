using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace OfficeHVAC.Messages
{
    public class RoomAvaliabilityMessage
    {
        public IActorRef RoomActor;

        public RoomAvaliabilityMessage(IActorRef roomActorRef)
        {
            this.RoomActor = roomActorRef;
        }
    }
}
