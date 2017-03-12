using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation;
using System;
using System.Threading.Tasks;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomSimulatorActor : RoomActor
    {
        private ICancelable Scheduler { get; set; }

        private ITemperatureSimulator TemperatureSimulator { get; }

        public RoomSimulatorActor(string roomName, ITemperatureSimulator temperatureSimulator, ActorPath companySupervisorActorPath) : base(roomName, companySupervisorActorPath)
        {
            this.TemperatureSimulator = temperatureSimulator;

            this.Sensors.Add(new SensorActorRef(Guid.NewGuid().ToString(), SensorType.Temperature, Self));
            
            //Temperature calculation 
            this.Receive<ChangeTemperature>(message => TemperatureSimulator.Temperature += message.DeltaT);

            this.Receive<TemperatureValueRequest>(message => Sender.Tell(new TemperatureValueMessage(TemperatureSimulator.Temperature)));

            //this.Receive<SetDesiredTemperature>(message => {
            //    foreach (ITemperatureDevice device in TemperatureSimulator.Devices)
            //        device.DesiredTemperature = message.DesiredTemperature;
            //});

            //this.Receive<SetDesiredMode>(message =>
            //{
            //    foreach (ITemperatureDevice device in TemperatureSimulator.Devices)
            //        device.SetActiveModeByName = message.DesiredMode;
            //});
        }

        protected override async Task<RoomStatusMessage> GenerateRoomStatus()
        {
            var msg = new RoomStatusMessage(RoomInfo, TemperatureSimulator.Temperature);
            return await Task.FromResult(msg);
        }

        protected override void PreStart()
        {
            var selection = Context.System.ActorSelection(this.CompanySupervisorActorPath.ToString());
            selection.Tell(new RoomAvaliabilityMessage(Self));

            Scheduler = Context.System
                .Scheduler
                .ScheduleTellRepeatedlyCancelable(
                    TimeSpan.FromMilliseconds(1000),
                    TimeSpan.FromMilliseconds(1000),
                    Self,
                    new RoomStatusRequest(), 
                    Self);
        }
    }
}
