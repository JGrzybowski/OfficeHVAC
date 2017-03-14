using Akka.Actor;
using OfficeHVAC.Models;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureSimulatorActor : ReceiveActor
    {
        private ITemperatureSimulator temperatureSimulator { get; }

        public TemperatureSimulatorActor(ITemperatureSimulator temperatureSimulator)
        {
            this.temperatureSimulator = temperatureSimulator;

            Receive<ParameterValue.Request>(msg =>
            {
                if (msg.ParameterType == SensorType.Temperature)
                    Sender.Tell(CheckTemperature());
                else
                    throw new InvalidMessageException();
            });
        }

        private IParameterValueMessage CheckTemperature()
        {
            var roomStatus = Sender.Ask<RoomStatus>(new RoomStatus.Request());

            return new ParameterValue(SensorType.Temperature, temperatureSimulator.Temperature);
        }
    }
}
