using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using NodaTime;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureControllerActor
{
    public class AddingDevice : TemperatureControllerActorTest
    {
        [Fact]
        public void AddsOneDeviceDeffinitionToTheActuatorStatus()
        {
            //Arrange
            var modelFake = GenerateModelFake();

            var now = Instant.FromUtc(2017, 03, 25, 08, 00);

            var device = new TemperatureDeviceDefinition()
            {
                Id = "Test dev",
                MaxPower = 1000,
                Modes = new List<ITemperatureMode>()
                {
                    new TemperatureMode()
                    {
                        Name = "Off",
                        Type = TemperatureModeType.Off,
                        TemperatureRange = new Range<double>(-100, 100)
                    },
                    new TemperatureMode()
                    {
                        Name = "Stabilization",
                        Type = TemperatureModeType.Stabilization,
                        PowerEfficiency = 0.99,
                        PowerConsumption = 0.1,
                        TemperatureRange = new Range<double>(-3, 3)
                    },
                    new TemperatureMode()
                    {
                        Name = "Eco",
                        Type = TemperatureModeType.Eco,
                        PowerEfficiency = 0.93,
                        PowerConsumption = 0.3,
                        TemperatureRange = new Range<double>(-100, 100)
                    }
                }
            };
            
            var status = new RoomStatus()
            {
            
            }.ToMessage();

            var testedActor = ActorOf(Actors.TemperatureControllerActor.Props(new string[0]));
            
            testedActor.Tell(new TimeChangedMessage(now));
            testedActor.Tell(status);
            testedActor.Tell(new TemperatureSimulation.SimpleTemperatureModel());
            
            IgnoreMessages<TemperatureControllerStatus>();
            testedActor.Tell(new DebugSubscribeMessage(TestActor), TestActor);
            IgnoreNoMessages();
            
            //Act
            testedActor.Tell(new AddDevice<ITemperatureDeviceDefinition>(device.ToMessage()), TestActor);
            
            //Assert
            var msg = FishForMessage<TemperatureControllerStatus>(s => s.Devices.Any());
            msg.Devices.ShouldHaveSingleItem().Id.ShouldBe(device.Id);
        }
    }
}