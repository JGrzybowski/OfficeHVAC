using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Models;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureSimulatorActor
{
    public class OnTemperatureRequest : TestKit
    {
        private string blackHoleAddress;

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
            var actor = ActorOfAsTestActorRef<Actors.TemperatureSimulatorActor>(
                Props.Create(() => new Actors.TemperatureSimulatorActor(temperatureSimulatorFake, blackHoleAddress)));

            var parameters = new ParameterValuesCollection()
            {
                new ParameterValue(SensorType.Temperature, 25)
            };

            var status = new RoomStatus()
            {
                Parameters = parameters,
                Volume = 20,
            };

            //Act
            actor.Tell(new ParameterValue.Request(SensorType.Temperature));
            var roomStatusRequest = ReceiveOne();
            LastSender.Tell(status.ToMessage());

            //Assert
            var temperatureMsg = ExpectMsg<IParameterValueMessage>();
            temperatureMsg.ParameterType.ShouldBe(SensorType.Temperature);
            temperatureMsg.Value.ShouldBe(expectedTemperature);
        }
    }
}
