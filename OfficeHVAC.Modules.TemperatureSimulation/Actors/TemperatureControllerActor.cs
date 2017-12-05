using Akka.Actor;
using NodaTime;
using OfficeHVAC.Components;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureControllerActor : SimulatingComponentActor<TemperatureControllerStatus, double>
    {
        protected ITemperatureModel TemperatureModel;
        protected bool ReceivedTemperatureModel => TemperatureModel != null;
        
        protected IActorRef JobShedulerActor { get; }         
        
        protected HashSet<TemperatureDevice> Devices { get; set; } = new HashSet<TemperatureDevice>();
        
        protected ExectionPlan ExecutionPlan { get; set; } = new ExectionPlan(new TemperatureJob[0]);
        
        protected override bool ReceivedInitialData() =>
            TimeStampInitialized && ReceivedTemperatureModel && ReceivedInitialRoomStatus;

        public TemperatureControllerActor(IEnumerable<string> subscriptionSources)
            : base(subscriptionSources)
        {
            JobShedulerActor = Context.ActorOf<TemperatureJobSheduler>();
        }

        protected override void Uninitialized()
        {
            Receive<ITemperatureModel>(model =>
            {
                TemperatureModel = model;
                InformAboutInternalState();
                if (ReceivedInitialData())
                    Become(Initialized);
            });

            RegisterDevicesManagementMessages();
            
            base.Uninitialized();
        }

        protected override void Initialized()
        {
            RegisterDevicesManagementMessages();

            Receive<IEnumerable<Requirement<double>>>(msg =>
            {
                var temperature = Convert.ToDouble(RoomStatus.Parameters[SensorType.Temperature].Value);
                var requirementsSet = new RequirementsSet<double, ITemperatureDeviceDefinition, ITemperatureModel>
                    (temperature, Timestamp, msg, Devices.Select(d => d.ToDefinition()).ToArray(), TemperatureModel, RoomStatus);
                JobShedulerActor.Tell(requirementsSet);
            });

            Receive<ExectionPlan>(msg =>
            {
                ExecutionPlan = msg;
                ControlDevices();
                InformAboutInternalState();
            });

            base.Initialized();
        }

        private void ControlDevices()
        {
            var nextJob = ExecutionPlan.Jobs.OrderBy(j => j.StartTime).FirstOrDefault();
            if (nextJob == null || nextJob?.StartTime > Timestamp)
                return;
            
            foreach (var device in Devices)
                device.SetActiveMode(nextJob.ModeType);
        }

        protected void RegisterDevicesManagementMessages()
        {
            Receive<AddDevice<ITemperatureDeviceDefinition>>(msg =>
            {
                if (Devices.Any(d => d.Id == msg.Definition.Id))
                    Devices.RemoveWhere(dev => dev.Id == msg.Definition.Id);
                
                Devices.Add(new TemperatureDevice()
                {
                    Id = msg.Definition.Id,
                    MaxPower = msg.Definition.MaxPower,
                    Modes = new ModesCollection(msg.Definition.Modes)
                });
                
                //TODO RECALCULATE JOBS!!
                InformAboutInternalState();
            });

            Receive<RemoveDevice>(msg =>
            {
                Devices.RemoveWhere(dev => dev.Id == msg.Id);
                //TODO RECALCULATE JOBS!!
                InformAboutInternalState();
            });
        }

        protected override void OnTimeUpdated(Duration timeDiff, Instant newTime)
        {
            base.OnTimeUpdated(timeDiff, newTime);
            ControlDevices();
        }

        protected override void InitializeFromRoomStatus(IRoomStatusMessage roomStatus)
        {
            RoomStatus = roomStatus;
            base.InitializeFromRoomStatus(roomStatus);
        }

        public static Props Props(IEnumerable<string> subscriptionSources) =>
            Akka.Actor.Props.Create(() => new TemperatureControllerActor(subscriptionSources));

        protected override TemperatureControllerStatus GenerateInternalState() =>
            new TemperatureControllerStatus(Id, ParameterValue, Timestamp, ThresholdBuffer,
                Devices.Select(d => d.ToStatus()).ToList());
    }

    public class TemperatureControllerStatus : SimulatingComponentStatus<double>
    {
        public TemperatureControllerStatus(string id, double parameter, Instant timestamp, Duration theresholdBuffer,
            IEnumerable<ITemperatureDeviceStatus> devices)
            : base(id, parameter, timestamp, theresholdBuffer)
        {
            Devices = devices;
        }

        public IEnumerable<ITemperatureDeviceStatus> Devices { get; }
    }
}