using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureJobSheduler : ReceiveActor
    {
        public TemperatureJobSheduler()
        {
            Receive<RequirementsSet<double, ITemperatureDeviceDefinition, ITemperatureModel>>(
                msg =>
                {
                    var exectionPlan = CreateExecutionPlan(msg);
                    Sender.Tell(exectionPlan);
                },
                msg => msg.Requirements.Any());
        }

        private ExectionPlan CreateExecutionPlan(RequirementsSet<double, ITemperatureDeviceDefinition, ITemperatureModel> requirementsSet)
        {
            var jobList = new List<TemperatureJob>();
            foreach (var earliestRequirement in requirementsSet.Requirements.OrderBy(r => r.StartTime))
            {
                var desiredTemperature = earliestRequirement.ExpextedParamValue;
                var time = earliestRequirement.StartTime - requirementsSet.InitialTime;
                var initialTemperature = requirementsSet.CurrentParamValue;
                
                var job = CalculateSingleJob(requirementsSet, initialTemperature, desiredTemperature, time, earliestRequirement);
                jobList.Add(job);
            }
            return new ExectionPlan(jobList);
        }

        private static TemperatureJob CalculateSingleJob(RequirementsSet<double, ITemperatureDeviceDefinition, ITemperatureModel> requirementsSet, double initialTemperature,
            double desiredTemperature, Duration time, Requirement<double> earliestRequirement)
        {
            var task = new TemperatureTask(initialTemperature, desiredTemperature, time);

            var temperatureDevices = requirementsSet.Actuators.ToList();

            var bestModeName =
                requirementsSet.Model.FindMostEfficientCombination(task, requirementsSet.RoomStatus, temperatureDevices);

            var timeNeeded = requirementsSet.Model.CalculateNeededTime(task.InitialTemperature, task.DesiredTemperature,
                temperatureDevices, bestModeName, requirementsSet.RoomStatus.Volume);
            var job = new TemperatureJob(bestModeName, desiredTemperature, earliestRequirement.StartTime - timeNeeded,
                earliestRequirement.StartTime);

            return job;
        }
    }

    public class ExectionPlan
    {
        public IEnumerable<TemperatureJob> Jobs { get; }
        public ExectionPlan(IEnumerable<TemperatureJob> jobs)
        {
            Jobs = jobs;
        }
    }
}