using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Modules.RoomSimulator.Factories;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Factories
{
    public class BridgeRoomActorPropsFactoryTests : TestKit
    {
        private readonly IRoomSimulatorActorPropsFactory roomSimulatorActorPropsFactoryFake;

        public BridgeRoomActorPropsFactoryTests()
        {
            roomSimulatorActorPropsFactoryFake = Substitute.For<IRoomSimulatorActorPropsFactory>();
            roomSimulatorActorPropsFactoryFake.Props().Returns(BlackHoleActor.Props);
        }

        [Fact]
        public void uses_RoomActorPropsFactory()
        {
            //Arrange
            var factory = new BridgeRoomActorPropsFactory(roomSimulatorActorPropsFactoryFake);

            //Act
            var bridgeProps = factory.Props();
            
            //Assert
            var bridgeActor = ActorOf(bridgeProps);
            roomSimulatorActorPropsFactoryFake.Received().Props();
        }

        [Fact]
        public void returns_RoomBridgeActor_props()
        {
            //Arrange
            var factory = new BridgeRoomActorPropsFactory(roomSimulatorActorPropsFactoryFake);

            //Act
            var resultProps = factory.Props();

            //Assert
            var bridgeActor = ActorOfAsTestActorRef<RoomSimulator.Actors.RoomBridgeActor>(resultProps);
            bridgeActor.UnderlyingActor.ShouldNotBeNull();
        }
    }
}
