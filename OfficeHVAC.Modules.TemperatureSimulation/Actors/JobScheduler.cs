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
    public class JobScheduler : ReceiveActor
    {
        private readonly ITimeSource timeSource;

        private readonly ITemperatureModel temperatureModel;

        public JobScheduler(ISimulatorModels models)
        {
            this.timeSource = models.TimeSource;
            this.temperatureModel = models.TemperatureModel;

            ReceiveAsync<Requirements>(
                async msg => Sender.Tell(CalculateNextJob(msg, await RequestStatus())),
                      msg => msg.Parameters.Contains(SensorType.Temperature));
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
                                           requirements.Deadline - timeSource.Now);
            else
                task = new TemperatureTask(preJob.DesiredTemperature,
                                    desiredTemperature,
                                    preJob.EndTime - timeSource.Now);

            var temperatureDevices =
                status.Devices
                    .Where(dev => dev is ITemperatureDeviceDefinition)
                    .Cast<ITemperatureDeviceDefinition>()
                    .ToList();

            var bestModeName = temperatureModel.FindMostEfficientCombination(task, status, temperatureDevices);
            var job = new TemperatureJob(bestModeName, desiredTemperature,
                                        requirements.Deadline - this.temperatureModel.CalculateNeededTime(task.InitialTemperature, task.DesiredTemperature, temperatureDevices, bestModeName, status.Volume),
                                        requirements.Deadline);

            return job;
        }

        private async Task<RoomStatus> RequestStatus()
        {
            return await Context.Sender.Ask<RoomStatus>(new RoomStatus.Request());
        }
    }
}

