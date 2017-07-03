using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomActor : ReceiveActor
    {
        public RoomStatus Status { get; } = new RoomStatus();

        protected ActorPath CompanySupervisorActorPath { get; }

        protected HashSet<ISensorActorRef> Sensors { get; } = new HashSet<ISensorActorRef>();

        protected HashSet<ISensorActorRef> Controllers { get; } = new HashSet<ISensorActorRef>();

        protected HashSet<IActorRef> Subscribers { get; } = new HashSet<IActorRef>();

        protected List<TemperatureJob> Jobs { get; } = new List<TemperatureJob>();

        protected HashSet<Requirements> Events { get; } = new HashSet<Requirements>();

        protected double StabilizationThreshold = 1.0;

        protected ISimulatorModels Models;

        public RoomActor(RoomStatus initialStatus, ActorPath companySupervisorActorPath, ISimulatorModels models) : this()
        {
            Status = initialStatus;
            CompanySupervisorActorPath = companySupervisorActorPath;
            Models = models;
        }

        public RoomActor()
        {
            Receive<ParameterValueMessage>(
                msg => Status.Parameters[msg.ParamType].Value = msg.Value,
                msg => msg.ParamType != SensorType.Unknown
            );

            //Subscribtion handling
            Receive<SubscribeMessage>(message => OnSubscribeMessage());
            Receive<SubscriptionTriggerMessage>(message => SendSubscribtionNewsletter());
            Receive<UnsubscribeMessage>(message => OnUnsubscribeMesssage());

            Receive<ParameterValue>(message => UpdateParameter(message));

            Receive<RoomStatus.Request>(message =>
            {
                var status = GenerateRoomStatus();
                Sender.Tell(status, Self);
            });

            Receive<Requirements>(message =>
            {
                Events.Add(message);
                Controllers.Single(s => s.Type == SensorType.Temperature).Actor.Tell(message);
            });

            Receive<TemperatureJob>(message =>
            {
                Jobs.Add(message);
                //var job = Jobs.OrderBy(j => j.EndTime).First();
                var job = message;
                ActivateTemperatureMode(job.ModeType, job.DesiredTemperature);
                SendSubscribtionNewsletter();
            });
        }

        protected virtual void OnUnsubscribeMesssage()
        {
            Subscribers.Remove(Sender);
        }

        protected virtual void SendSubscribtionNewsletter()
        {
            var status = GenerateRoomStatus();
            foreach (var subscriber in Subscribers)
                subscriber.Tell(status, Self);
        }

        protected virtual void OnSubscribeMessage()
        {
            Subscribers.Add(Sender);
            Sender.Tell(GenerateRoomStatus());
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

        protected override void PreStart()
        {
            var selection = Context.System.ActorSelection(CompanySupervisorActorPath.ToString());
            selection.Tell(new RoomAvaliabilityMessage(Self));

            base.PreStart();
        }

        protected override void PostRestart(Exception reason) { }

        protected virtual IRoomStatusMessage GenerateRoomStatus()
        {
            return Status.ToMessage();
        }
    }
}
