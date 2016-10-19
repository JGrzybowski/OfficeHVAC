using Akka.Actor;
using OfficeHVAC.Simulators;

namespace OfficeHVAC.Actors
{
    public class RoomSimulatorActor : ReceiveActor
    {
        public string RoomName { get; set; }
        public ITemperatureSimulator TemperatureSimulator { get; set; }

        public RoomSimulatorActor(string roomName, ITemperatureSimulator temperatureSimulator)
        {
            this.RoomName = roomName;
            this.TemperatureSimulator = temperatureSimulator;
        }
    }
}
