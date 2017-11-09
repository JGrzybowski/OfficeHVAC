using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using Shouldly;
using System.Threading;
using OfficeHVAC.Models.Subscription;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.RoomBridgeActor
{
    public class Constructor : TestKit
    {
        private readonly ViewModels.IRoomViewModel _viewModel =
            Substitute.For<ViewModels.IRoomViewModel>();

        private readonly Props _echoActorProps;

        public Constructor()
        {
            _echoActorProps = EchoActor.Props(this, false);
        }
        
        [Fact]
        public void creates_room_actor()
        {
            //Arrange & Act
            var bridge = ActorOf(() => new RoomSimulator.Actors.RoomBridgeActor(_viewModel, _echoActorProps), "bridge");
            ExpectMsg<SubscribeMessage>();
            Thread.Sleep(500);

            //Assert
            var roomActor = Sys.ActorSelection(bridge.Path.Child(RoomSimulator.Actors.RoomBridgeActor.RoomActorName));
            roomActor.Tell("Hello");
            ExpectMsg<string>(msg => msg.ShouldBe("Hello"));
        }
    }
}
