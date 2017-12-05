using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Models.Subscription;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomActor : ReceiveActor
    {
        public RoomStatus Status { get; } = new RoomStatus();

        protected HashSet<ISensorActorRef> Sensors { get; } = new HashSet<ISensorActorRef>();

        protected HashSet<ISensorActorRef> Actuators { get; } = new HashSet<ISensorActorRef>();

        protected IActorRef SubscribersManager { get; private set; }

        protected double StabilizationThreshold = 1.0;

        public RoomActor(RoomStatus initialStatus, ICanTell parentContact) : this()
        {
            Status = initialStatus;
            
            parentContact.Tell(new RoomAvaliabilityMessage(Self), Self);
        }

        public RoomActor()
        {
            SubscribersManager = Context.ActorOf<SubscriptionActor>();
            
            Receive<ParameterValueMessage>(
                msg => Status.Parameters[msg.ParamType].Value = msg.Value,
                msg => msg.ParamType != SensorType.Unknown
            );

            Receive<SensorAvaliableMessage>(msg => AddSensor(Sender, msg.SensorType, msg.SensorId));
            
            //Subscribtion handling
            Receive<SubscribeMessage>(message =>
            {
                SubscribersManager.Forward(message);
                message.Subscriber.Tell(GenerateRoomStatus());
            });
            Receive<UnsubscribeMessage>(message => SubscribersManager.Forward(message));
            Receive<SubscriptionTriggerMessage>(message => { SendSubscribtionNewsletter(); });
            
            Receive<ParameterValue>(message => UpdateParameter(message));

            Receive<RoomStatus.Request>(message =>
            {
                var status = GenerateRoomStatus();
                Sender.Tell(status);
            });

            Receive<AddDevice<ITemperatureDeviceDefinition>>(def =>
            {
                foreach (var actuator in Actuators)
                    actuator.Actor.Forward(def);
            });

            Receive<RemoveDevice>(msg =>
            {
                foreach (var actuator in Actuators)
                    actuator.Actor.Forward(msg);
            });

            // JOB SCHEDULING SECTION
//            Receive<Requirements>(message =>
//            {
//                Events.Add(message);
//                Actuators.Single(s => s.Type == SensorType.Temperature).Actor.Tell(message);
//            });
//
//            Receive<TemperatureJob>(message =>
//            {
//                Jobs.Add(message);
//                //var job = Jobs.OrderBy(j => j.EndTime).First();
//                var job = message;
//                ActivateTemperatureMode(job.ModeType, job.DesiredTemperature);
//                SendSubscribtionNewsletter();
//            });
        }

        protected virtual void AddSensor(IActorRef sensorActorRef, SensorType sensorType, string sensorId)
        {
            var sensorRef = new SensorActorRef(sensorId, sensorType, sensorActorRef);
            Sensors.Add(sensorRef);
            sensorActorRef.Tell(new SubscribeMessage(Self)); 
        }

        protected virtual void AddActuator(IActorRef actuatorRef, SensorType actuatorType, string actuatorId)
        {
            var actuatorDeviceRef = new SensorActorRef(actuatorId, actuatorType, actuatorRef);
            Actuators.Add(actuatorDeviceRef);
            actuatorRef.Tell(new SubscribeMessage(Self)); 
        }

        protected virtual void SendSubscribtionNewsletter()
        {
            var status = GenerateRoomStatus();
            SubscribersManager.Tell(new SendToSubscribersMessage(status));
        }

        protected virtual void UpdateParameter(ParameterValue paramValue)
        {
            if(Status.Parameters.Contains(paramValue.ParameterType))
                Status.Parameters[paramValue.ParameterType].Value = paramValue.Value;
            else 
                Status.Parameters.Add(paramValue.Clone() as ParameterValue);

            //if (paramValue.ParameterType == SensorType.Temperature)
            //{
            //    var temperature = Convert.ToDouble(paramValue.Value);
            //    if (Jobs.Any() && Math.Abs(Jobs.First().DesiredTemperature - temperature) < StabilizationThreshold)
            //        ActivateTemperatureMode(TemperatureModeType.Stabilization, Jobs.First().DesiredTemperature);
            //}
            SendSubscribtionNewsletter();
        }

        protected virtual void ActivateTemperatureMode(TemperatureModeType mode, double desiredTemperature)
        {
            foreach (var device in Status.TemperatureDevices.Where(dev => dev is ITemperatureDevice).Cast<ITemperatureDevice>())
            {
                device.SetActiveMode(mode);
                device.DesiredTemperature = desiredTemperature;
            }
        }

        protected virtual IRoomStatusMessage GenerateRoomStatus()
        {
            return Status.ToMessage();
        }

        public static Props Props(RoomStatus status, ICanTell parentContact)
            => Akka.Actor.Props.Create(() => new RoomActor(status.Clone(), parentContact));
    }
}
