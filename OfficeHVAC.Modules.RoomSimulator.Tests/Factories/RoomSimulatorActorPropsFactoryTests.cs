using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Modules.RoomSimulator.Actors;
using OfficeHVAC.Modules.RoomSimulator.Factories;
using OfficeHVAC.Modules.TemperatureSimulation;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using Shouldly;
using System;
using OfficeHVAC.Models;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Factories
{
    public class RoomSimulatorActorPropsFactoryTests : TestKit
    {
        private readonly IActorPathBuilder _pathBuilderFake;
        private readonly ITemperatureSimulatorFactory _temperatureSimulatorFactoryFake;

        public RoomSimulatorActorPropsFactoryTests()
        {
            IActorRef blackHoleActor = ActorOfAsTestActorRef<BlackHoleActor>(BlackHoleActor.Props);

            _pathBuilderFake = Substitute.For<IActorPathBuilder>();
            _pathBuilderFake.ActorPath().Returns(blackHoleActor.Path);

            ITemperatureSimulator temperatureSimulatorFake = Substitute.For<ITemperatureSimulator>();
            temperatureSimulatorFake.GetTemperature(Arg.Any<IRoomStatusMessage>()).Returns(28f);

            _temperatureSimulatorFactoryFake = Substitute.For<ITemperatureSimulatorFactory>();
            _temperatureSimulatorFactoryFake.TemperatureSimulator().Returns(temperatureSimulatorFake);
        }

        [Fact]
        public void returns_RoomActor_props()
        {
            //Arrange
            var propsBuilder = new RoomSimulatorActorPropsFactory(_pathBuilderFake, _temperatureSimulatorFactoryFake, Substitute.For<ISimulatorModels>()) { RoomName = "Room 101" };

            //Act
            var resultProps = propsBuilder.Props();

            //Assert
            var roomActor = ActorOfAsTestActorRef<RoomSimulatorActor>(resultProps);
            roomActor.UnderlyingActor.ShouldNotBeNull();
        }

        [Fact]
        public void constructs_props_using_ActorPathBuilder()
        {
            //Arrange
            var propsBuilder = new RoomSimulatorActorPropsFactory(_pathBuilderFake, _temperatureSimulatorFactoryFake, Substitute.For<ISimulatorModels>()) { RoomName = "Room 101" };

            //Act
            var resultProps = propsBuilder.Props();

            //Assert
            _pathBuilderFake.Received().ActorPath();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        public void throws_ArgumentException_when_RoomName_is_empty(string emptyRoomName)
        {
            //Arrange
            var propsBuilder = new RoomSimulatorActorPropsFactory(_pathBuilderFake, _temperatureSimulatorFactoryFake, Substitute.For<ISimulatorModels>()) { RoomName = emptyRoomName };

            //Act
            var ex = Should.Throw<ArgumentException>(() => propsBuilder.Props());

            //Assert
            ex.ParamName.ShouldBe(nameof(RoomSimulatorActorPropsFactory.RoomName));
        }
    }
}
