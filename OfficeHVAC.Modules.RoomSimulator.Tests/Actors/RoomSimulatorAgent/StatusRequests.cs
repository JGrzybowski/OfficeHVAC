using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NodaTime;
using NSubstitute;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Actors.RoomSimulatorAgent
{
    public class StatusRequests : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const float TemperatureInRoom = 20f;

        private static ITemperatureSimulatorFactory GenerateTemperatureSimulatorFake()
        {
            var simulatorFake = Substitute.For<ITemperatureSimulator>();
            simulatorFake.ChangeTemperature(Arg.Any<IRoomStatusMessage>(), Arg.Any<Duration>())
                         .Returns(TemperatureInRoom);

            var factoryFake = Substitute.For<ITemperatureSimulatorFactory>();
            factoryFake.TemperatureSimulator().Returns(simulatorFake);

            return factoryFake;
        }

        private Props RoomActorProps() =>
            Props.Create(() => new RoomSimulator.Actors.RoomSimulatorActor(
                new RoomStatus()
                {
                    Name = TestRoomName,
                    Parameters = new ParameterValuesCollection()
                    {
                        new ParameterValue(SensorType.Temperature, TemperatureInRoom)
                    }
                },
                ActorOf(BlackHoleActor.Props).Path,
                GenerateTemperatureSimulatorFake(),
                Substitute.For<ISimulatorModels>()
        ));

        [Fact]
        public void responds_with_room_status_when_requested()
        {
            //Arrange
            var actor = ActorOf(RoomActorProps());

            //Act
            actor.Tell(new RoomStatus.Request());

            //Assert
            ExpectMsg<IRoomStatusMessage>(msg =>
            {
                msg.Name.ShouldBe(TestRoomName);
                msg.Parameters[SensorType.Temperature].Value.ShouldBe(TemperatureInRoom);
            });
        }
    }
}
