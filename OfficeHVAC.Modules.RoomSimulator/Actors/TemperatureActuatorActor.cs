using Akka.Actor;

namespace OfficeHVAC.Modules.RoomSimulator.Actors {
    public class TemperatureActuatorActor : ReceiveActor
    {
        public static Props Props() => Akka.Actor.Props.Create(() => new TemperatureActuatorActor());

        public class Status { }
    }
}