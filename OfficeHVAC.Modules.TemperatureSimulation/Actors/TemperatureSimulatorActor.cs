using Akka.Actor;
using OfficeHVAC.Models;
using System.Threading.Tasks;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureSimulatorActor : ReceiveActor
    {
        private ITemperatureSimulator temperatureSimulator { get; }

        public TemperatureSimulatorActor(ITemperatureSimulator temperatureSimulator)
        {
            this.temperatureSimulator = temperatureSimulator;

            ReceiveAsync<ParameterValue.Request>(
                async msg => Sender.Tell(await CheckTemperature()),
                msg => msg.ParameterType == SensorType.Temperature
            );
        }

        private async Task<IParameterValueMessage> CheckTemperature()
        {
            var roomStatus = await Sender.Ask<IRoomStatusMessage>(new RoomStatus.Request());

            temperatureSimulator.RoomVolume = roomStatus.Volume;
            var temperature = temperatureSimulator.GetTemperature(roomStatus);

            return new ParameterValue(SensorType.Temperature, temperature);
        }
    }
}
