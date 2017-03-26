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

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureControllerActor : ReceiveActor
    {
        private readonly ITimeSource timeSource;

        private readonly ITemperatureModel temperatureModel;

        public readonly HashSet<TemperatureDevice> Devices = new HashSet<TemperatureDevice>();

        public TemperatureControllerActor(ITemperatureModel temperatureModel, ITimeSource timeSource, IEnumerable<ITemperatureDeviceDefinition> devicesDefinitions)
        {
            this.timeSource = timeSource;
            this.temperatureModel = temperatureModel;

            foreach (var devDef in devicesDefinitions)
                AddNewDevice(devDef);

            Receive<AddTemperatureDevice>(msg => AddNewDevice(msg.Definition));
            Receive<RemoveDevice>(msg => RemoveDevice(msg.Id));
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

        private async void CalculateNextJob(TemperatureTask task, IEnumerable<TemperatureDevice> devices, IRoomStatusMessage status)
        {
            throw new NotImplementedException();
            var nextTask = await Context.Parent.Ask<TemperatureTask>(new GetNextTask());
            var modesNames = devices
                                .Select(dev => dev.ModesNames)
                                .Cast<IEnumerable<string>>()
                                .Aggregate((resultModes, newModes) => resultModes.Intersect(newModes));
            var actualTemperature = (double)status.Parameters[SensorType.Temperature].Value;

            var bestMode = modesNames
                .Select(modeName => new
                {
                    Time = temperatureModel.CalculateNeededTime(actualTemperature, nextTask.Temperature, devices, modeName, status.Area),
                    ModeName = modeName
                })
                .Where(timeMode => (timeSource.Now + timeMode.Time) < nextTask.Deadline)
                .Select(timeMode => new
                {
                    Power = devices.Sum(dev => dev.CalculatePowerConsumption(timeMode.ModeName, timeMode.Time)),
                    timeMode.Time,
                    timeMode.ModeName
                })
                .MinBy(powerMode => powerMode.Power);
            //NextJob = new TemperatureJob(bestMode.ModeName, nextTask.Deadline - bestMode.Time, nextTask.Deadline);
        }
    }
}

