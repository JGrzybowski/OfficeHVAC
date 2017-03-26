using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using System;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomSimulatorActor : RoomActor
    {
        private ICancelable Scheduler { get; set; }

        private readonly ITemperatureSimulatorFactory temperatureSimulatorFactory;

        public RoomSimulatorActor(RoomStatus initialStatus, ActorPath companySupervisorActorPath, ITemperatureSimulatorFactory temperatureSimulatorFactory)
            : base(initialStatus, companySupervisorActorPath)
        {
            this.temperatureSimulatorFactory = temperatureSimulatorFactory;

            this.Sensors.Add(
                new SensorActorRef(Guid.NewGuid().ToString(),
                SensorType.Temperature,
                Context.ActorOf(this.PrepareTemperatureSimulatorActorProps(initialStatus))
            ));

            this.Receive<ChangeTemperature>(
                msg => Status.Parameters[SensorType.Temperature].Value = Convert.ToDouble(Status.Parameters[SensorType.Temperature].Value) + msg.DeltaT,
                msg => Status.Parameters.Contains(SensorType.Temperature));


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

        protected Props PrepareTemperatureSimulatorActorProps(RoomStatus initialStatus)
        {
            if (initialStatus.Parameters.Contains(SensorType.Temperature))
                this.temperatureSimulatorFactory.InitialTemperature = Convert.ToDouble(initialStatus.Parameters[SensorType.Temperature].Value);

            var props = Props.Create<TemperatureSimulatorActor>(
                () => new TemperatureSimulatorActor(temperatureSimulatorFactory.TemperatureSimulator())
            );

            return props;
        }
    }
}
