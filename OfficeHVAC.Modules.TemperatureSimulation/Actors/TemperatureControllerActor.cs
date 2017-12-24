using Akka.Actor;
using NodaTime;
using OfficeHVAC.Components;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureControllerActor : SimulatingComponentActor<TemperatureControllerStatus, double>
    {
        protected ITemperatureModel TemperatureModel;
        protected bool ReceivedTemperatureModel => TemperatureModel != null;
        
        protected IActorRef JobShedulerActor { get; }         
        
        protected HashSet<TemperatureDevice> Devices { get; set; } = new HashSet<TemperatureDevice>();

        protected IEnumerable<Requirement<double>> Requirements { get; set; } = new List<Requirement<double>>(); 
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
                Requirements = msg; 
                JobShedulerActor.Tell(GenerateRequirementsSet());
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
                
                if (Requirements.Any())
                    JobShedulerActor.Tell(GenerateRequirementsSet());

                InformAboutInternalState();
            });

            Receive<RemoveDevice>(msg =>
            {
                Devices.RemoveWhere(dev => dev.Id == msg.Id);
                if (Requirements.Any())
                    JobShedulerActor.Tell(GenerateRequirementsSet());
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

        protected override void UpdateRoomStatus(IRoomStatusMessage roomStatus)
        {
            Debug.WriteLine($"Status From: {Sender.Path}");
            base.UpdateRoomStatus(roomStatus);
        }

        protected RequirementsSet<double, ITemperatureDeviceDefinition, ITemperatureModel> GenerateRequirementsSet()
        {
            var reqSet = new RequirementsSet<double, ITemperatureDeviceDefinition, ITemperatureModel>
            (
                Convert.ToDouble(RoomStatus.Parameters[SensorType.Temperature].Value),
                Timestamp,
                Requirements,
                Devices.Select(d => d.ToDefinition()).ToArray(),
                TemperatureModel,
                RoomStatus
            );

            return reqSet;
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