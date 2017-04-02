using NodaTime;
using OfficeHVAC.Models.Devices;
using System.Collections.Generic;
using System.Linq;
using OfficeHVAC.Messages;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureControllerActor
{
    public class OnRemoveDevice : TemperatureControllerActorTest
    {
        [Fact]
        public void removes_specified_dev_controller()
        {
            //Arrange
            var modelFake = GenerateModelFake();
            var offMode = new TemperatureMode() { Name = "Off" };
            var devices = new List<ITemperatureDeviceDefinition>()
            {
                new TemperatureDeviceDefinition() {Id = "Id1", Modes = new [] {offMode}},
                new TemperatureDeviceDefinition() {Id = "Id2", Modes = new [] {offMode}}
            };

            var controller = ActorOfAsTestActorRef(() =>
                new Actors.JobScheduler(
                    modelFake,
                    new TimeSourceFake(new Instant()),
                    devices));

            //Act
            controller.Tell(new RemoveDevice("Id1"));

            //Assert
            controller.UnderlyingActor.Devices.Count.ShouldBe(1);
            controller.UnderlyingActor.Devices.First().Id.ShouldBe("Id2");
        }
    }
}
