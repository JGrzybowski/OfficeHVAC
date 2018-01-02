using Akka.Actor;
using NodaTime;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomActor : ReceiveActor
    {
        public RoomStatus Status { get; } = new RoomStatus();

        protected HashSet<ISensorActorRef> Sensors { get; } = new HashSet<ISensorActorRef>();

        protected HashSet<ISensorActorRef> Actuators { get; } = new HashSet<ISensorActorRef>();

        protected List<Expectation> Expectations { get; set; } = new List<Expectation>();

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
            //Receive<Expectation>(expect =>
            //{
            //    Expectations.Add(expect);
            //    SplitExpectations(Expectations);
            //});

            Receive<IEnumerable<Expectation>>(expectations =>
            {
                Expectations = new List<Expectation>(expectations);
                SplitExpectations(Expectations);
            });

            Receive<TemperatureControllerStatus>(msg =>
            {
                Status.TemperatureDevices = new HashSet<ITemperatureDeviceStatus>(msg.Devices);
                SendSubscribtionNewsletter();
            });
        }

        private void SplitExpectations(IEnumerable<Expectation> expectations)
        {
            var requirements = 
                expectations
                    .SelectMany(e => e.ExpectedParametersValues.Select(epv => new { e.From, e.Till, epv.ParameterType, epv.Value}))
                    .GroupBy(ex => ex.ParameterType)
                    .Where(ex => ex.Key != SensorType.Unknown);

            foreach (var requirement in requirements)
            {
                var controller = Actuators.FirstOrDefault(ctlr => ctlr.Type == requirement.Key);
                if (controller != null)
                {
                    //HACK! works as long as parameters values are doubles!
                    var reqs = requirement.Select(exp => new Requirement<double>(exp.From, exp.Till, Convert.ToDouble(exp.Value)));
                    controller.Actor.Tell(reqs);
                }
            }
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

            SendSubscribtionNewsletter();
        }

        protected virtual IRoomStatusMessage GenerateRoomStatus()
        {
            return Status.ToMessage();
        }

        public static Props Props(RoomStatus status, ICanTell parentContact)
            => Akka.Actor.Props.Create(() => new RoomActor(status.Clone(), parentContact));
    }

    public class Expectation
    {
        public Instant From { get; }
        public Instant Till { get; }
        public IEnumerable<ParameterValue> ExpectedParametersValues { get; }

        public Expectation(Instant @from, Instant till, IEnumerable<ParameterValue> expectedParametersValues)
        {
            From = @from;
            Till = till;
            ExpectedParametersValues = expectedParametersValues;
        }
    }
}
