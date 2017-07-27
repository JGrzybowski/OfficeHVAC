using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using System;
using System.Linq;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomSimulatorActor : RoomActor
    {
        private ICancelable Scheduler { get; set; }

        private readonly ITemperatureSimulatorFactory temperatureSimulatorFactory;

        public RoomSimulatorActor(RoomStatus initialStatus, ActorPath companySupervisorActorPath, ITemperatureSimulatorFactory temperatureSimulatorFactory,
                                  ISimulatorModels models)
            : base(initialStatus, models)
        {
            this.temperatureSimulatorFactory = temperatureSimulatorFactory;

            Sensors.Add(new SensorActorRef(
                Guid.NewGuid().ToString(),
                SensorType.Temperature,
                Context.ActorOf(this.PrepareTemperatureSimulatorActorProps(initialStatus))));

            var tempController = Context.ActorOf(JobScheduler.Props(models, initialStatus.TimeStamp));
            Controllers.Add(new SensorActorRef(
                Guid.NewGuid().ToString(),
                SensorType.Temperature,
                tempController));

            this.Receive<ChangeTemperature>(
                msg => Status.Parameters[SensorType.Temperature].Value = Convert.ToDouble(Status.Parameters[SensorType.Temperature].Value) + msg.DeltaT,
                msg => Status.Parameters.Contains(SensorType.Temperature));

            //this.Receive<TimeChangedMessage>(
            //    msg => , 
            //    msg =>
            //);

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
            Subscribers.Add(Sensors.Single(s => s.Type == SensorType.Temperature).Actor);
            SendSubscribtionNewsletter();
            base.PreStart();
        }

        protected Props PrepareTemperatureSimulatorActorProps(RoomStatus initialStatus)
        {
            if (initialStatus.Parameters.Contains(SensorType.Temperature))
                this.temperatureSimulatorFactory.InitialTemperature = Convert.ToDouble(initialStatus.Parameters[SensorType.Temperature].Value);

            var props = TemperatureSimulatorActor.Props(this.Status, new object() as ISimulatorModels);

            return props;
        }
    }
}
