using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using System;
using System.Linq;
using System.Windows.Navigation;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomSimulatorActor : RoomActor
    {
        private ICancelable Scheduler { get; set; }

        private readonly ITemperatureSimulatorFactory temperatureSimulatorFactory;

        public RoomSimulatorActor(RoomStatus initialStatus, ActorPath companySupervisorActorPath,ActorPath timeSimulatorActorPath, ActorPath tempSimulatorModelActorPath)
            : this(initialStatus, companySupervisorActorPath.ToStringWithoutAddress(),timeSimulatorActorPath.ToStringWithoutAddress(), tempSimulatorModelActorPath.ToStringWithoutAddress()) { }

        public RoomSimulatorActor(RoomStatus initialStatus, string companySupervisorActorPath, string timeSimulatorActorPath, string tempSimulatorModelActorPath)
            : base(initialStatus, Context.System.ActorSelection(companySupervisorActorPath))
        {     
            
//            var tempController = Context.ActorOf(JobScheduler.Props(models, initialStatus.TimeStamp));
//            Controllers.Add(new SensorActorRef(
//                Guid.NewGuid().ToString(),
//                SensorType.Temperature,
//                tempController));

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
            StatusSubscribers.Add(Sensors.Single(s => s.Type == SensorType.Temperature).Actor);
            SendSubscribtionNewsletter();
            base.PreStart();
        }

        protected Props PrepareTemperatureSimulatorActorProps(RoomStatus initialStatus, string timeActorPath, string tempActorPath)
        {
            if (initialStatus.Parameters.Contains(SensorType.Temperature))
                this.temperatureSimulatorFactory.InitialTemperature = Convert.ToDouble(initialStatus.Parameters[SensorType.Temperature].Value);

            var props = TemperatureSimulatorActor.Props(this.Status, timeActorPath, tempActorPath);

            return props;
        }

        public static Props Props(RoomStatus initialStatus, string companySupervisorActorPath, string timeSimulatorActorPath, string tempSimulatorModelActorPath)
        {
            return Akka.Actor.Props.Create(() => new RoomSimulatorActor(initialStatus.Clone(), companySupervisorActorPath, timeSimulatorActorPath, tempSimulatorModelActorPath));
        }
        
        public static Props Props(RoomStatus initialStatus, ActorPath companySupervisorActorPath, ActorPath timeSimulatorActorPath, ActorPath tempSimulatorModelActorPath)
        {
            return Akka.Actor.Props.Create(() => new RoomSimulatorActor(initialStatus.Clone(), companySupervisorActorPath, timeSimulatorActorPath, tempSimulatorModelActorPath));
        }
    }
}
