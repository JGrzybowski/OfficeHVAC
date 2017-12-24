using Akka.Actor;
using NodaTime;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureControllerActor
{
    public class RemovingDevice : TemperatureControllerActorTest
    {
        [Fact]
        public void RemovesDeviceDefinitionFromDevice()
        {
            var modelFake = GenerateModelFake();

            var now = Instant.FromUtc(2017, 03, 25, 08, 00);

            var device = new TemperatureDeviceDefinition
            (
                id : "Test dev",
                maxPower : 1000,
                modes : new List<ITemperatureMode>()
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
            );
            
            var device2 =new TemperatureDeviceDefinition
            (
                id : "Test dev2",
                maxPower : 1000,
                modes : new List<ITemperatureMode>()
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
            );
            
            var status = new RoomStatus()
            {
            
            }.ToMessage();

            var testedActor = ActorOf(Actors.TemperatureControllerActor.Props(new string[0]));
            
            testedActor.Tell(new TimeChangedMessage(now));
            testedActor.Tell(status);
            testedActor.Tell(new TemperatureSimulation.SimpleTemperatureModel());

            testedActor.Tell(new AddDevice<ITemperatureDeviceDefinition>(device.ToMessage()));
            testedActor.Tell(new AddDevice<ITemperatureDeviceDefinition>(device2.ToMessage()));
            
            IgnoreMessages<TemperatureControllerStatus>();
            testedActor.Tell(new DebugSubscribeMessage(TestActor));
            IgnoreNoMessages();
            
            //Act
            testedActor.Tell(new RemoveDevice(device.Id));
            
            //Assert
            var msg = FishForMessage<TemperatureControllerStatus>(m => m.Devices.Count()<2);
            msg.Devices.ShouldHaveSingleItem().Id.ShouldBe(device2.Id);
        }
    }
}