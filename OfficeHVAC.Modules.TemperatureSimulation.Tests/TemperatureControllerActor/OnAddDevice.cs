﻿using NodaTime;
using OfficeHVAC.Models.Devices;
using Shouldly;
using System.Collections.Generic;
using OfficeHVAC.Messages;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureControllerActor
{
    public class OnAddDevice : TemperatureControllerActorTest
    {
        [Fact]
        public void initializes_new_device_controller()
        {
            //Arrange 
            var temperatureModelFake = GenerateModelFake();
            var timeFake = new TimeSourceFake(new Instant());

            var controller = ActorOfAsTestActorRef(() =>
                new Actors.TemperatureControllerActor(
                    temperatureModelFake,
                    timeFake,
                    new List<ITemperatureDeviceDefinition>()));

            var message = new AddTemperatureDevice(
                new TemperatureDeviceDefinition()
                {
                    Id = "Id",
                    MaxPower = 2000,
                    Modes = new ITemperatureMode[0]
                });

            //Act
            controller.Tell(message);

            //Assert
            controller.UnderlyingActor.Devices.Count.ShouldBe(1);
        }
    }
}
