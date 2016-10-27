﻿using System;
using Akka.Actor;
using Akka.Configuration;
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
        // Actor Fakes
        private readonly IActorRef _blackHoleActor;
        // Factories Fakes
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
            var resultProps = propsBuilder.Props();

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
            var resultProps = propsBuilder.Props();

            //Assert
            _pathBuilderFake.Received().ActorPath();
        }

        [Fact]
        public void constructs_props_using_TemperatureSimulatorBuilder()
        {
            //Arrange
            var propsBuilder = new RoomSimulatorActorPropsFactory(_pathBuilderFake, _temperatureSimulatorFactoryFake) { RoomName = "Room 101" };

            //Act
            var resultProps = propsBuilder.Props();

            //Assert
            _temperatureSimulatorFactoryFake.Received().TemperatureSimulator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        public void throws_ArgumentException_when_RoomName_is_empty(string emptyRoomName)
        {
            //Arrange
            var propsBuilder = new RoomSimulatorActorPropsFactory(_pathBuilderFake, _temperatureSimulatorFactoryFake){RoomName = emptyRoomName};

            //Act
            var ex = Should.Throw<ArgumentException>(() => propsBuilder.Props());

            //Assert
            ex.ParamName.ShouldBe(nameof(RoomSimulatorActorPropsFactory.RoomName));
        }
    }
}