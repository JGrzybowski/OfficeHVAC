using Akka.Actor;
using MoreLinq;
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
        public double StabilizationLimit { get; } = 1.0;

        protected ITemperatureModel TemperatureModel;
        protected bool ReceivedTemperatureModel => TemperatureModel != null;
        
        protected IActorRef JobShedulerActor { get; }         
        
        protected HashSet<TemperatureDevice> Devices { get; set; } = new HashSet<TemperatureDevice>();

        protected List<Requirement<double>> Requirements { get; set; } = new List<Requirement<double>>(); 
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
                Requirements = new List<Requirement<double>>(msg); 
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
            if (!ExecutionPlan.Jobs.Any())
            {
                SetDevicesToMode(TemperatureModeType.Off);
                return;
            }

            var nextJob = ExecutionPlan.Jobs.OrderBy(j => j.StartTime).First();
            if (nextJob.StartTime > Timestamp)
                return;

            SetDevicesToMode(nextJob.ModeType, nextJob.DesiredTemperature);
        }

        private void StabilizeDevices()
        {
            foreach (var device in Devices)
            {
                device.SetActiveMode(
                    device.Modes.Contains(TemperatureModeType.Stabilization)
                    ? TemperatureModeType.Stabilization
                    : TemperatureModeType.Off);
            }
        }

        private void SetDevicesToMode(TemperatureModeType mode, double? temperature = null)
        {
            foreach(var device in Devices)
            {
                device.SetActiveMode(mode);
                device.DesiredTemperature = temperature ?? device.DesiredTemperature;
            }
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
            var removed = Requirements.RemoveAll(req => req.EndTime <= newTime);
            if (removed > 0)
            {
                if(Requirements.Count > 0)
                    JobShedulerActor.Tell(GenerateRequirementsSet());
                else 
                    ExecutionPlan = new ExectionPlan(new TemperatureJob[0]);
            }

            ControlDevices();

            base.OnTimeUpdated(timeDiff, newTime);
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

            var nextJob = ExecutionPlan.Jobs.OrderBy(j => j.StartTime).FirstOrDefault();
            if (nextJob == null) return;

            var tValue = roomStatus.Parameters.TryGet(SensorType.Temperature)?.Value;
            if (tValue == null) return;

            var T = Convert.ToDouble(tValue);
            if (Math.Abs(nextJob.DesiredTemperature - T) < StabilizationLimit)
                StabilizeDevices();
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