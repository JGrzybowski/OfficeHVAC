using Akka.Actor;
using Akka.TestKit.Xunit2;
using NodaTime;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureSimulatorActor
{
    public class OnTimeSignal : TestKit
    {
        public OnTimeSignal()
        {

        }
        
        [Fact]
        public void changes_temperature_value()
        {
            //Arrange
            var temperatureSimulator = new TemperatureSimulator(25, new TemperatureSimulation.SimpleTemperatureModel());

            var now = Instant.FromDateTimeUtc(DateTime.UtcNow);
            
            var parameters = new ParameterValuesCollection()
            {
                new ParameterValue(SensorType.Temperature, 25.0)
            };

            var turboMode = new TemperatureMode() { Name = "Turbo", Type = TemperatureModeType.Turbo, PowerConsumption = 1, PowerEfficiency = 0.5 };
            var device = new TemperatureDevice()
            {
                MaxPower = 5000,
                DesiredTemperature = 27
            };
            device.Modes.Add(turboMode);
            device.SetActiveMode(TemperatureModeType.Turbo);

            var status = new RoomStatus()
            {
                Parameters = parameters,
                Volume = 20.0,
                TimeStamp = now,
                TemperatureDevices = new HashSet<ITemperatureDeviceStatus>{device.ToStatus()}
            };
            
            var actor = ActorOfAsTestActorRef<Actors.TemperatureSimulatorActor>(
                Props.Create(() => new Actors.TemperatureSimulatorActor(temperatureSimulator, new string[0])));

            actor.Tell(new TimeChangedMessage(now));
            actor.Tell(status.ToMessage());
            actor.Tell(new TemperatureSimulation.SimpleTemperatureModel());
            
            //Act
            actor.Tell(new TimeChangedMessage(now + Duration.FromMinutes(30)));

            //Assert
            actor.Tell(new ParameterValue.Request(SensorType.Temperature));

            var temperatureMsg = FishForMessage<IParameterValueMessage>(msg => msg.ParameterType == SensorType.Temperature);
            temperatureMsg.ParameterType.ShouldBe(SensorType.Temperature);

            var temperature = Convert.ToDouble(temperatureMsg.Value);
            temperature.ShouldBeGreaterThan(25);
        }
    }
}
