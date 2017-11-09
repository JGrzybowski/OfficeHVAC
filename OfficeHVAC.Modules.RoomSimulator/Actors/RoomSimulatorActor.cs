using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using System;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation.Messages;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomSimulatorActor : RoomActor
    {
        public RoomSimulatorActor(RoomStatus initialStatus, ActorPath companySupervisorActorPath)
            : this(initialStatus, companySupervisorActorPath.ToStringWithoutAddress()) { }

        public RoomSimulatorActor(RoomStatus initialStatus, string companySupervisorActorPath)
            : base(initialStatus, Context.System.ActorSelection(companySupervisorActorPath))
        {     
//            var tempController = Context.ActorOf(JobScheduler.Props(models, initialStatus.TimeStamp));
//            Controllers.Add(new SensorActorRef(
//                Guid.NewGuid().ToString(),
//                SensorType.Temperature,
//                tempController));

            Receive<ChangeTemperature>(
                msg => Status.Parameters[SensorType.Temperature].Value = Convert.ToDouble(Status.Parameters[SensorType.Temperature].Value) + msg.DeltaT,
                msg => Status.Parameters.Contains(SensorType.Temperature));

            Receive<AddTemperatureSensorMessage>(
                msg =>
                {
                    var props = PrepareTemperatureSimulatorActorProps(msg.TimeActorPath, msg.TemperatureParamerersActorPath);
                    var tSim = Context.ActorOf(props, "temperatureSimulator");
                    AddSensor(tSim, SensorType.Temperature, msg.SensorId);
                    Sender.Tell(tSim);
                });
            
            Receive<TimeChangedMessage>(
                msg =>
                {
                    Status.TimeStamp = msg.Now;
                    foreach (var sensor in Sensors)
                        sensor.Actor.Tell(msg);
                },
                msg => msg.Now > Status.TimeStamp
            );

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

        protected override void AddSensor(IActorRef sensorRef, SensorType sensorType, string sensorId)
        {
            base.AddSensor(sensorRef, sensorType, sensorId);
            sensorRef.Tell(new TimeChangedMessage(Status.TimeStamp));
            sensorRef.Tell(GenerateRoomStatus());
        }

        protected Props PrepareTemperatureSimulatorActorProps(string timeActorPath, string tempActorPath)
        {
            var props = TemperatureSimulatorActor.Props(Status, timeActorPath, tempActorPath);
            return props;
        }

        public static Props Props(RoomStatus initialStatus, string companySupervisorActorPath)
        {
            return Akka.Actor.Props.Create(() => new RoomSimulatorActor(initialStatus.Clone(), companySupervisorActorPath));
        }
        
        public static Props Props(RoomStatus initialStatus, ActorPath companySupervisorActorPath)
        {
            return Akka.Actor.Props.Create(() => new RoomSimulatorActor(initialStatus.Clone(), companySupervisorActorPath));
        }
    }
}
