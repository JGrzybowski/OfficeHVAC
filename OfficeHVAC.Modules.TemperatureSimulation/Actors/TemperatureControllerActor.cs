﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Threading.Tasks;
using Akka.Actor;
using NodaTime;
using OfficeHVAC.Components;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureControllerActor : SimulatingComponentActor<TemperatureControllerStatus, double>
    {
        protected ITemperatureModel TemperatureModel;
        protected bool ReceivedTemperatureModel => TemperatureModel != null;

        protected HashSet<TemperatureDeviceDefinition> Devices { get; set; } =
            new HashSet<TemperatureDeviceDefinition>();

        protected override bool ReceivedInitialData() =>
            TimeStampInitialized && ReceivedTemperatureModel && ReceivedInitialRoomStatus;

        public TemperatureControllerActor(IEnumerable<string> subscriptionSources)
            : base(subscriptionSources) { }

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

            ReceiveAsync<Requirements>(
                async msg => Sender.Tell(CalculateNextJob(msg, await RequestStatus())),
                msg => msg.Parameters.Contains(SensorType.Temperature));

            base.Initialized();
        }

        protected void RegisterDevicesManagementMessages()
        {
            Receive<AddDevice<ITemperatureDeviceDefinition>>(msg =>
            {
                if (Devices.Any(d => d.Id == msg.Definition.Id))
                    Devices.RemoveWhere(dev => dev.Id == msg.Definition.Id);
                Devices.Add(new TemperatureDeviceDefinition(msg.Definition.Id, msg.Definition.MaxPower, msg.Definition.Modes));
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

        protected override void InitializeFromRoomStatus(IRoomStatusMessage roomStatus)
        {
            RoomStatus = roomStatus;
            base.InitializeFromRoomStatus(roomStatus);
        }

        private TemperatureJob CalculateNextJob(Requirements requirements, IRoomStatusMessage status)
        {
            var desiredTemperature = Convert.ToDouble(requirements.Parameters[SensorType.Temperature].Value);
            var preJob = status.Jobs
                .Where(j => j.EndTime < requirements.Deadline)
                .OrderBy(j => requirements.Deadline - j.EndTime)
                .FirstOrDefault();

            TemperatureTask task;
            if (preJob == null)
                task = new TemperatureTask(Convert.ToDouble(status.Parameters[SensorType.Temperature].Value),
                    desiredTemperature,
                    requirements.Deadline - Timestamp);
            else
                task = new TemperatureTask(preJob.DesiredTemperature,
                    desiredTemperature,
                    preJob.EndTime - Timestamp);

            var temperatureDevices =
                status.TemperatureDevices
                    .Where(dev => dev is ITemperatureDeviceDefinition)
                    .Cast<ITemperatureDeviceDefinition>()
                    .ToList();

            var bestModeName = TemperatureModel.FindMostEfficientCombination(task, status, temperatureDevices);
            var job = new TemperatureJob(bestModeName, desiredTemperature,
                requirements.Deadline - TemperatureModel.CalculateNeededTime(task.InitialTemperature,
                    task.DesiredTemperature, temperatureDevices, bestModeName, status.Volume),
                requirements.Deadline);

            return job;
        }

        private async Task<RoomStatus> RequestStatus()
        {
            return await Context.Sender.Ask<RoomStatus>(new RoomStatus.Request(), TimeSpan.FromSeconds(5));
        }

        public static Props Props(IEnumerable<string> subscriptionSources) =>
            Akka.Actor.Props.Create(() => new TemperatureControllerActor(subscriptionSources));

        protected override TemperatureControllerStatus GenerateInternalState() =>
            new TemperatureControllerStatus(Id, ParameterValue, Timestamp, Threshold,
                Devices.Select(d => d.ToMessage()).ToList());
    }

    public class TemperatureControllerStatus : SimulatingComponentStatus<double>
    {
        public TemperatureControllerStatus(string id, double parameter, Instant timestamp, Duration theresholdBuffer,
            IEnumerable<ITemperatureDeviceDefinition> devices)
            : base(id, parameter, timestamp, theresholdBuffer)
        {
            Devices = devices;
        }

        public IEnumerable<ITemperatureDeviceDefinition> Devices { get; }
    }
}