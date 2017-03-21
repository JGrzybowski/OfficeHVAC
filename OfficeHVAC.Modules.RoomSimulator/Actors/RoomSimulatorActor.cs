using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using System;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomSimulatorActor : RoomActor
    {
        private ICancelable Scheduler { get; set; }

        public RoomSimulatorActor(string roomName, ActorPath companySupervisorActorPath, ParameterValuesCollection parameters)
            : base(new RoomInfo() { Name = roomName }, companySupervisorActorPath, parameters)
        {
            this.Sensors.Add(new SensorActorRef(Guid.NewGuid().ToString(), SensorType.Temperature, Self));

            this.Receive<ChangeTemperature>(
                msg => Parameters[SensorType.Temperature].Value = Convert.ToDouble(Parameters[SensorType.Temperature].Value) + msg.DeltaT, 
                msg => Parameters.Contains(SensorType.Temperature));


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

        protected override void PreStart()
        {
            Scheduler = Context.System
                .Scheduler
                .ScheduleTellRepeatedlyCancelable(
                    TimeSpan.FromMilliseconds(1000),
                    TimeSpan.FromMilliseconds(1000),
                    Self,
                    new SubscriptionTriggerMessage(),
                    Self);

            base.PreStart();
        }
    }
}
