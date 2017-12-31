using NodaTime;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureControllerActor
{
    public class OnTemperatureTask : TemperatureControllerActorTest
    {
        [Fact]
        public void sets_up_the_most_efficient_setting_as_closest_job()
        {
            //Arrange
            var modelFake = new TemperatureSimulation.SimpleTemperatureModel();

            var now = Instant.FromUtc(2017, 03, 25, 08, 00);
            var timefake = new TimeSourceFake(now);
            var deadline = Instant.FromUtc(2017, 03, 25, 15, 00);

            var initialTemperature = 25.0;
            var desiredTemperature = 21.0;

            var status = new RoomStatus()
            {
                Volume = 150,
                Parameters = new ParameterValuesCollection()
                {
                    new ParameterValue(SensorType.Temperature, initialTemperature)
                }
            }.ToMessage();

            var offMode = new TemperatureMode() { Name = "Off", Type = TemperatureModeType.Off};
            var ecoMode = new TemperatureMode() { Name = "Eco", Type = TemperatureModeType.Eco, PowerEfficiency = 0.95, PowerConsumption = 0.3 };
            var turboMode = new TemperatureMode() { Name = "Turbo", Type = TemperatureModeType.Turbo, PowerEfficiency = 0.5, PowerConsumption = 1.0 };

            var devices = new List<ITemperatureDeviceDefinition>()
            {
                new TemperatureDeviceDefinition
                (
                    id : "Test dev",
                    maxPower : 2000,
                    modes : new[] { offMode, ecoMode, turboMode } 
                )
            };

            var controller = ActorOfAsTestActorRef(() => new Actors.TemperatureControllerActor(new string[0]));
            
            //Initialize Controller
            controller.Tell(new TimeChangedMessage(now));
            controller.Tell(status);
            controller.Tell(new TemperatureSimulation.SimpleTemperatureModel());
            controller.Tell(new AddDevice<ITemperatureDeviceDefinition>(devices.Single()));

            var requiremnent = new Requirement<double>(now + Duration.FromMinutes(30), now + Duration.FromHours(5), desiredTemperature);

            //Act
            controller.Tell(new List<Requirement<double>>{requiremnent});
            controller.Tell(new DebugSubscribeMessage(TestActor));

            for (int i = 0; i * 5 < 30; i++)
            {
                now = now + Duration.FromMinutes(5);
                controller.Tell(new TimeChangedMessage(now));
            }

            //Assert
            var msg = FishForMessage<TemperatureControllerStatus>(m => m.Devices.Any(d => d.ActiveModeType != TemperatureModeType.Off), TimeSpan.FromMinutes(10));
            msg.ShouldNotBeNull();
        }
    }
}
