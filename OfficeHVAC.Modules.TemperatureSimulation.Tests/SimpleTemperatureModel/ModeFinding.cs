using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.SimpleTemperatureModel
{
    public class ModeFinding
    {
        [Fact]
        public void sets_up_the_most_efficient_setting_as_closest_job()
        {
            //Arrange
            var time = Duration.FromHours(8);

            var initialTemperature = 25;
            var desiredTemperature = 21;

            var status = new RoomStatus()
            {
                Volume = 35
            }.ToMessage();

            var offMode = new TemperatureMode() { Name = "Off" };
            var ecoMode = new TemperatureMode() { Name = "Eco", PowerEfficiency = 0.95, PowerConsumption = 0.3 };
            var turboMode = new TemperatureMode() { Name = "Turbo", PowerEfficiency = 0.50, PowerConsumption = 1.0 };

            var devices = new List<ITemperatureDeviceDefinition>()
            {
                new TemperatureDeviceDefinition() { Id = "Id1", Modes = new[] { offMode, ecoMode, turboMode }, MaxPower = 2500 }
            };

            var task = new TemperatureTask(initialTemperature, desiredTemperature, time);

            var model = new TemperatureSimulation.SimpleTemperatureModel();

            //Act
            var bestMode = model.FindMostEfficientCombination(task, status, devices);

            //Assert
            bestMode.ShouldBe(ecoMode.Name);
        }
    }
}
