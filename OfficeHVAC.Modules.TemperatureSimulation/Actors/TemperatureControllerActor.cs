using Akka.Actor;
using MoreLinq;
using OfficeHVAC.Factories.Propses;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using NodaTime;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureControllerActor : ReceiveActor
    {
        private readonly ITimeSource timeSource;

        private readonly ITemperatureModel temperatureModel;

        public readonly HashSet<TemperatureDevice> Devices = new HashSet<TemperatureDevice>();

        public readonly List<TemperatureJob> Jobs = new List<TemperatureJob>();

        public TemperatureControllerActor(ITemperatureModel temperatureModel, ITimeSource timeSource, IEnumerable<ITemperatureDeviceDefinition> devicesDefinitions)
        {
            this.timeSource = timeSource;
            this.temperatureModel = temperatureModel;

            foreach (var devDef in devicesDefinitions)
                AddNewDevice(devDef);

            Receive<AddTemperatureDevice>(msg => AddNewDevice(msg.Definition));
            Receive<RemoveDevice>(msg => RemoveDevice(msg.Id));
            ReceiveAsync<Requirements>(
                async msg => CalculateNextJob(msg, Devices, await RequestStatus()),
                      msg => msg.Parameters.Contains(SensorType.Temperature));
        }

        private void AddNewDevice(ITemperatureDeviceDefinition devDef)
        {
            var dev = new TemperatureDevice()
            {
                Id = devDef.Id,
                MaxPower = devDef.MaxPower,
                Modes = new HashSet<ITemperatureMode>(devDef.Modes),
            };
            Devices.Add(dev);
        }

        private void RemoveDevice(string id)
        {
            foreach (var dev in Devices.Where(dev => dev.Id == id))
                dev.TurnOff();

            Devices.RemoveWhere(dev => dev.Id == id);
        }

        private void CalculateNextJob(Requirements requirements, IEnumerable<ITemperatureDeviceDefinition> devices, IRoomStatusMessage status)
        {
            var desiredTemperature = (double)requirements.Parameters[SensorType.Temperature].Value;
            var preJob = this.Jobs.Where(j => j.EndTime < requirements.Deadline)
                                  .MinBy(j => requirements.Deadline - j.EndTime);

            TemperatureTask task;
            if (preJob == null)
                task = new TemperatureTask((double)status.Parameters[SensorType.Temperature].Value,
                                           desiredTemperature,
                                           requirements.Deadline - timeSource.Now);
            else
                task = new TemperatureTask(preJob.DesiredTemperature,
                                    desiredTemperature,
                                    preJob.EndTime - timeSource.Now);

            var bestModeName = temperatureModel.FindMostEfficientCombination(task, status, devices);
            var job = new TemperatureJob(bestModeName, desiredTemperature,
                                        requirements.Deadline - this.temperatureModel.CalculateNeededTime(task.InitialTemperature, task.DesiredTemperature, devices, bestModeName, status.Volume),
                                        requirements.Deadline);

            AddJob(job);
        }

        private async Task<RoomStatus> RequestStatus()
        {
            return await Context.Sender.Ask<RoomStatus>(new RoomStatus.Request());
        }

        private void AddJob(TemperatureJob job)
        {
            Jobs.Add(job);
            Jobs.Sort(new TemperatureJob.StartDateComparer());
        }
    }
}

