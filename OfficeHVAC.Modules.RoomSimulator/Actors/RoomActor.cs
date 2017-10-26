using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using OfficeHVAC.Models.Subscription;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomActor : ReceiveActor
    {
        public RoomStatus Status { get; } = new RoomStatus();

        protected HashSet<ISensorActorRef> Sensors { get; } = new HashSet<ISensorActorRef>();

        protected HashSet<ISensorActorRef> Controllers { get; } = new HashSet<ISensorActorRef>();

        protected IActorRef SubscribersManager { get; private set; }

        protected List<TemperatureJob> Jobs { get; } = new List<TemperatureJob>();

        protected HashSet<Requirements> Events { get; } = new HashSet<Requirements>();

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

            // JOB SCHEDULING SECTION
//            Receive<Requirements>(message =>
//            {
//                Events.Add(message);
//                Controllers.Single(s => s.Type == SensorType.Temperature).Actor.Tell(message);
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

        protected virtual void SendSubscribtionNewsletter()
        {
            var status = GenerateRoomStatus();
            SubscribersManager.Tell(new SendToSubscribersMessage(status));
        }

        protected virtual void UpdateParameter(ParameterValue paramValue)
        {
            this.Status.Parameters[paramValue.ParameterType].Value = paramValue.Value;
            if (paramValue.ParameterType == SensorType.Temperature)
            {
                var temperature = Convert.ToDouble(paramValue.Value);
                if (Jobs.Any() && Math.Abs(Jobs.First().DesiredTemperature - temperature) < StabilizationThreshold)
                    ActivateTemperatureMode(TemperatureModeType.Stabilization, Jobs.First().DesiredTemperature);
            }
            SendSubscribtionNewsletter();
        }

        protected virtual void ActivateTemperatureMode(TemperatureModeType mode, double desiredTemperature)
        {
            foreach (var device in Status.Devices.Where(dev => dev is ITemperatureDevice).Cast<ITemperatureDevice>())
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
