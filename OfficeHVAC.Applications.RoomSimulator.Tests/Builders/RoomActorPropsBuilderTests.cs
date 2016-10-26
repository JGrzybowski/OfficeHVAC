using System;
using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using NSubstitute.Extensions;
using OfficeHVAC.Actors;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Factories.Propses;
using OfficeHVAC.Factories.Simulators.Temperature;
using OfficeHVAC.Factories.TimeSources;
using OfficeHVAC.Models;
using OfficeHVAC.Simulators;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.Builders
{
    public class RoomActorPropsBuilderTests : TestKit
    {
        private readonly IActorRef _blackHoleActor;

        private readonly ITimeSource _timeSourceFake;
        private readonly ITimeSourceFactory _timeSourceFactoryFake;

        private readonly IActorPathBuilder _pathBuilderFake;
        private readonly ITemperatureSimulatorFactory _temperatureSimulatorFactoryFake;
        private readonly ITemperatureSimulator _temperatureSimulatorFake;

        public RoomActorPropsBuilderTests()
        {
            _blackHoleActor = ActorOfAsTestActorRef<BlackHoleActor>(BlackHoleActor.Props);

            _timeSourceFake = Substitute.For<ITimeSource>();
            _timeSourceFake.Now.Returns(new DateTime(1999, 12, 31));

            _timeSourceFactoryFake = Substitute.For<ITimeSourceFactory>();
            _timeSourceFactoryFake.TimeSource().Returns(_timeSourceFake);

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
            var propsBuilder = new RoomActorPropsFactory(_pathBuilderFake, _timeSourceFactoryFake,_temperatureSimulatorFactoryFake);
            
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
            var propsBuilder = new RoomActorPropsFactory(_pathBuilderFake, _timeSourceFactoryFake, _temperatureSimulatorFactoryFake);

            //Act
            var resultProps = propsBuilder.Build();

            //Assert
            _pathBuilderFake.Received().ActorPath();
        }

        [Fact]
        public void constructs_props_using_TimeSourceBuilder()
        {
            //Arrange
            var propsBuilder = new RoomActorPropsFactory(_pathBuilderFake, _timeSourceFactoryFake, _temperatureSimulatorFactoryFake);

            //Act
            var resultProps = propsBuilder.Build();

            //Assert
            _timeSourceFactoryFake.Received().TimeSource();
        }

        [Fact]
        public void constructs_props_using_TemperatureSimulatorBuilder()
        {
            //Arrange
            var propsBuilder = new RoomActorPropsFactory(_pathBuilderFake, _timeSourceFactoryFake, _temperatureSimulatorFactoryFake);

            //Act
            var resultProps = propsBuilder.Build();

            //Assert
            _temperatureSimulatorFactoryFake.Received().TemperatureSimulator();
        }

    }
}
