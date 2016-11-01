using System.Threading;
using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Factories.Configs;
using OfficeHVAC.Messages;
using OfficeHVAC.Modules.RoomSimulator.Factories;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.RoomBridgeActor
{
    public class Constructor : TestKit
    {
        private readonly ViewModels.RoomSimulatorViewModel _viewModel =
            Substitute.For<ViewModels.RoomSimulatorViewModel>(
                Substitute.For<IConfigBuilder>(),
                Substitute.For<IBridgeRoomActorPropsFactory>()
            );

        private readonly Props _echoActorProps;

        public Constructor()
        {
            _echoActorProps = EchoActor.Props(this, false);
        }
        
        [Fact]
        public void creates_room_actor()
        {
            //Arrange & Act
            var bridge = ActorOf(() => new Actors.RoomBridgeActor(_viewModel, _echoActorProps), "bridge");
            ExpectMsg<SubscribeMessage>();
            Thread.Sleep(500);

            //Assert
            var roomActor = Sys.ActorSelection(bridge.Path.Child(Actors.RoomBridgeActor.RoomActorName));
            roomActor.Tell("Hello");
            ExpectMsg<string>(msg => msg.ShouldBe("Hello"));
        }
    }
}
