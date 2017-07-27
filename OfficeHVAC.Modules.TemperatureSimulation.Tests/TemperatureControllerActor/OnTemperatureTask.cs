using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System.Collections.Generic;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureControllerActor
{
    public class OnTemperatureTask : TemperatureControllerActorTest
    {
        [Fact]
        public void sets_up_the_most_efficient_setting_as_closest_job()
        {
            //Arrange
            var modelFake = GenerateModelFake();

            var now = Instant.FromUtc(2017, 03, 25, 08, 00);
            var timefake = new TimeSourceFake(now);
            var deadline = Instant.FromUtc(2017, 03, 25, 15, 00);

            var initialTemperature = 25;
            var desiredTemperature = 21;

            var status = new RoomStatus()
            {

            }.ToMessage();

            var offMode = new TemperatureMode() { Name = "Off" };
            var ecoMode = new TemperatureMode() { Name = "Eco", PowerEfficiency = 0.95, PowerConsumption = 0.3 };
            var turboMode = new TemperatureMode() { Name = "Turbo", PowerEfficiency = 0.5, PowerConsumption = 1.0 };

            var devices = new List<ITemperatureDeviceDefinition>()
            {
                new TemperatureDeviceDefinition() { Id = "Id1", Modes = new[] { offMode, ecoMode, turboMode } }
            };

            //var models = new SimulatorModels(modelFake);

            //var controller = ActorOfAsTestActorRef(() => new Actors.JobScheduler(models));

            //Act
            //controller.Ask<RoomStatus.Request>(new TemperatureTask(desiredTemperature, new Instant() + Duration.FromHours(36)), TimeSpan.FromSeconds(3));
            //LastSender.Tell

            //Assert


        }
    }
}
