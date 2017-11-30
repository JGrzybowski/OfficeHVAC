using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.RoomSimulator.Messages;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using System;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomSimulatorActor : RoomActor
    {
        public RoomSimulatorActor(RoomStatus initialStatus, ActorPath companySupervisorActorPath)
            : this(initialStatus, companySupervisorActorPath.ToStringWithoutAddress()) { }

        public RoomSimulatorActor(RoomStatus initialStatus, string companySupervisorActorPath)
            : base(initialStatus, Context.System.ActorSelection(companySupervisorActorPath))
        {     
            Receive<ChangeTemperature>(
                msg => Status.Parameters[SensorType.Temperature].Value = Convert.ToDouble(Status.Parameters[SensorType.Temperature].Value) + msg.DeltaT,
                msg => Status.Parameters.Contains(SensorType.Temperature));

            Receive<AddTemperatureSensorMessage>(
                msg =>
                {
                    var props = PrepareTemperatureSimulatorActorProps(msg.SensorId, msg.TimeActorPath, msg.TemperatureParamerersActorPath);
                    var tSim = Context.ActorOf(props, "temperatureSimulator");
                    AddSensor(tSim, SensorType.Temperature, msg.SensorId);
                    Sender.Tell(tSim);
                });

            Receive<AddTemperatureActuatorMessage>(msg =>
            {
                var actuatorProps = PrepareTemperatureActuatorProps();
                var acuatorRef = Context.ActorOf(actuatorProps, "temperatureActuator");
                AddActuator(acuatorRef, SensorType.Temperature, msg.Id);
                Sender.Tell(acuatorRef);
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

            Receive<SetTemperature>(msg => UpdateParameter(new ParameterValue(SensorType.Temperature, msg.Temperature)));

            Receive<SetRoomVolume>(msg =>
            {
                Status.Volume = msg.Volume;
                SendSubscribtionNewsletter();
            });
        }

        protected override void AddSensor(IActorRef sensorRef, SensorType sensorType, string sensorId)
        {
            base.AddSensor(sensorRef, sensorType, sensorId);
            sensorRef.Tell(new TimeChangedMessage(Status.TimeStamp));
            sensorRef.Tell(GenerateRoomStatus());
        }

        protected override void AddActuator(IActorRef actuatorRef, SensorType actuatorType, string actuatorId)
        {
            base.AddActuator(actuatorRef, actuatorType, actuatorId);
            actuatorRef.Tell(new TimeChangedMessage(Status.TimeStamp));
            actuatorRef.Tell(GenerateRoomStatus());
        }

        protected Props PrepareTemperatureSimulatorActorProps(string id, string timeActorPath, string tempActorPath) => 
            TemperatureSimulatorActor.Props(Status, timeActorPath, tempActorPath);

        protected Props PrepareTemperatureActuatorProps() => 
            TemperatureActuatorActor.Props();

        public static Props Props(RoomStatus initialStatus, string companySupervisorActorPath) => 
            Akka.Actor.Props.Create(() => new RoomSimulatorActor(initialStatus.Clone(), companySupervisorActorPath));

        public static Props Props(RoomStatus initialStatus, ActorPath companySupervisorActorPath) => 
            Akka.Actor.Props.Create(() => new RoomSimulatorActor(initialStatus.Clone(), companySupervisorActorPath));
    }
}
