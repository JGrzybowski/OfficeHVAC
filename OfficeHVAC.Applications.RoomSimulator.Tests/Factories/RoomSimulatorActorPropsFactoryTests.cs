using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Actors;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Factories.Simulators.Temperature;
using OfficeHVAC.Simulators;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.Factories
{
    public class RoomSimulatorActorPropsFactoryTests : TestKit
    {
        private readonly IActorRef _blackHoleActor;

        private readonly IActorPathBuilder _pathBuilderFake;
        private readonly ITemperatureSimulatorFactory _temperatureSimulatorFactoryFake;
        private readonly ITemperatureSimulator _temperatureSimulatorFake;

        public RoomSimulatorActorPropsFactoryTests()
        {
            _blackHoleActor = ActorOfAsTestActorRef<BlackHoleActor>(BlackHoleActor.Props);

            _pathBuilderFake = Substitute.For<IActorPathBuilder>();
            _pathBuilderFake.ActorPath().Returns(_blackHoleActor.Path);

            _temperatureSimulatorFake = Substitute.For<ITemperatureSimulator>();
            _temperatureSimulatorFake.Temperature.Returns(28f);

            _temperatureSimulatorFactoryFake = Substitute.For<ITemperatureSimulatorFactory>();
            _temperatureSimulatorFactoryFake.TemperatureSimulator().Returns(_temperatureSimulatorFake);
        }
        
        [Fact]
        public void returns_RoomActor_props()
        {
            //Arrange
            var propsBuilder = new RoomSimulatorActorPropsFactory(_pathBuilderFake, _temperatureSimulatorFactoryFake) {RoomName = "Room 101"};
            
            //Act
            var resultProps = propsBuilder.Build();

            //Assert
            var roomActor = ActorOfAsTestActorRef<RoomSimulatorActor>(resultProps);
            roomActor.UnderlyingActor.ShouldNotBeNull();
        }

        [Fact]
        public void constructs_props_using_ActorPathBuilder()
        {
            //Arrange
            var propsBuilder = new RoomSimulatorActorPropsFactory(_pathBuilderFake, _temperatureSimulatorFactoryFake) { RoomName = "Room 101" };

            //Act
            var resultProps = propsBuilder.Build();

            //Assert
            _pathBuilderFake.Received().ActorPath();
        }

        [Fact]
        public void constructs_props_using_TemperatureSimulatorBuilder()
        {
            //Arrange
            var propsBuilder = new RoomSimulatorActorPropsFactory(_pathBuilderFake, _temperatureSimulatorFactoryFake) { RoomName = "Room 101" };

            //Act
            var resultProps = propsBuilder.Build();

            //Assert
            _temperatureSimulatorFactoryFake.Received().TemperatureSimulator();
        }
    }
}
