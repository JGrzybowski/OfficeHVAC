using Akka.Actor;
using OfficeHVAC.Models;
using System;
using System.Threading.Tasks;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureSimulatorActor : ReceiveActor
    {
        private IRoomStatusMessage roomStatus;

        private ICancelable Scheduler { get; set; }
        private ITemperatureSimulator temperatureSimulator { get; }

        public TemperatureSimulatorActor(ITemperatureSimulator temperatureSimulator)
        {
            this.temperatureSimulator = temperatureSimulator;

            ReceiveAsync<ParameterValue.Request>(
                async msg => Sender.Tell(await CheckTemperature()),
                msg => msg.ParameterType == SensorType.Temperature
            );

            Receive<IRoomStatusMessage>(msg => roomStatus = msg);
        }

        protected override void PreStart()
        {
            Scheduler = Context.System
                .Scheduler
                .ScheduleTellRepeatedlyCancelable(
                    TimeSpan.FromMilliseconds(1000),
                    TimeSpan.FromMilliseconds(15000),
                    Self,
                    new ParameterValue.Request(SensorType.Temperature),
                    Context.Parent);

            base.PreStart();
        }

        private async Task<IParameterValueMessage> CheckTemperature()
        {
            temperatureSimulator.RoomVolume = roomStatus.Volume;
            var temperature = temperatureSimulator.GetTemperature(roomStatus);

            return new ParameterValue(SensorType.Temperature, temperature);
        }
    }
}
