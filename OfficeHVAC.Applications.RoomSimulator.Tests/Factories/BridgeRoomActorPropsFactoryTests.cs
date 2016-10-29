using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.Factories
{
    public class BridgeRoomActorPropsFactoryTests :TestKit
    {
        private readonly IRoomSimulatorActorPropsFactory _roomSimulatorActorPropsFactoryFake;
        private readonly ViewModels.IRoomViewModel _viewModelFake;

        public BridgeRoomActorPropsFactoryTests()
        {
            _roomSimulatorActorPropsFactoryFake = Substitute.For<IRoomSimulatorActorPropsFactory>();
            _roomSimulatorActorPropsFactoryFake.Props().Returns(BlackHoleActor.Props);

            _viewModelFake = Substitute.For<ViewModels.IRoomViewModel>();
        }

        [Fact]
        public void uses_RoomActorPropsFactory()
        {
            //Arrange
            var factory = new BridgeRoomActorPropsFactory(_roomSimulatorActorPropsFactoryFake, _viewModelFake);

            //Act
            var bridgeProps = factory.Props();
            
            //Assert
            var bridgeActor = ActorOf(bridgeProps);
            _roomSimulatorActorPropsFactoryFake.Received().Props();
        }

        [Fact]
        public void returns_RoomBridgeActor_props()
        {
            //Arrange
            var factory = new BridgeRoomActorPropsFactory(_roomSimulatorActorPropsFactoryFake, _viewModelFake);

            //Act
            var resultProps = factory.Props();

            //Assert
            var bridgeActor = ActorOfAsTestActorRef<Actors.RoomBridgeActor>(resultProps);
            bridgeActor.UnderlyingActor.ShouldNotBeNull();
        }
    }
}
