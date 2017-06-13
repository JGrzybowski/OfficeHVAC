using Akka.Actor;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Models;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureSimulatorActor
{
    public class OnTemperatureRequest : TestKit
    {
        [Fact]
        public void sends_room_status_request()
        {
            //Arrange
            var temperatureSimulatorFake = Substitute.For<ITemperatureSimulator>();
            var actor = ActorOfAsTestActorRef<Actors.TemperatureSimulatorActor>(
                Props.Create<Actors.TemperatureSimulatorActor>(temperatureSimulatorFake));

            //Act		
            actor.Tell(new ParameterValue.Request(SensorType.Temperature));

            //Assert
            ExpectMsg<RoomStatus.Request>();
        }

        [Fact]
        public void sends_data_based_on_TemperatureSimulator()
        {
            //Arrange
            int expectedTemperature = 27;
            var temperatureSimulatorFake = Substitute.For<ITemperatureSimulator>();
            temperatureSimulatorFake.GetTemperature(Arg.Any<IRoomStatusMessage>()).Returns(expectedTemperature);
            var actor = ActorOfAsTestActorRef<Actors.TemperatureSimulatorActor>(
                Props.Create<Actors.TemperatureSimulatorActor>(temperatureSimulatorFake));

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
