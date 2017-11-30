using System;
using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NodaTime;
using NSubstitute;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureSimulatorActor
{
    public class OnTemperatureRequest : TestKit
    {
        private readonly string blackHoleAddress;

        public OnTemperatureRequest()
        {
            blackHoleAddress = ActorOf<BlackHoleActor>().Path.ToStringWithoutAddress();
        }
        
        [Fact]
        public void sends_data_based_on_TemperatureSimulator()
        {
            //Arrange
            int expectedTemperature = 27;
            var temperatureSimulatorFake = Substitute.For<ITemperatureSimulator>();
            temperatureSimulatorFake.Temperature = expectedTemperature;

            var now = Instant.FromDateTimeUtc(DateTime.UtcNow);
            
            var parameters = new ParameterValuesCollection()
            {
                new ParameterValue(SensorType.Temperature, 25.0)
            };

            var status = new RoomStatus()
            {
                Parameters = parameters,
                Volume = 20.0,
                TimeStamp = now
            };
            
            var actor = ActorOfAsTestActorRef<Actors.TemperatureSimulatorActor>(
                Props.Create(() => new Actors.TemperatureSimulatorActor(temperatureSimulatorFake, new string[0])));

            actor.Tell(new TimeChangedMessage(now));
            actor.Tell(status.ToMessage());
            actor.Tell(new TemperatureSimulation.SimpleTemperatureModel());
            
            //Act
            actor.Tell(new ParameterValue.Request(SensorType.Temperature));
            
            //Assert
            var temperatureMsg = FishForMessage<IParameterValueMessage>(msg => msg.ParameterType ==SensorType.Temperature);
            temperatureMsg.ParameterType.ShouldBe(SensorType.Temperature);
            temperatureMsg.Value.ShouldBe(expectedTemperature);
        }
    }
}
