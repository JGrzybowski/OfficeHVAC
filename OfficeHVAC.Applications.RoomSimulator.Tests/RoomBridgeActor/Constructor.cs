using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.Configs;
using Shouldly;
using System.Threading;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomBridgeActor
{
    public class Constructor : TestKit
    {
        private readonly ViewModels.RoomViewModel _viewModel =
            Substitute.For<ViewModels.RoomViewModel>(
                Substitute.For<IRoomSimulatorActorPropsFactory>(),
                Substitute.For<IConfigBuilder>()
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
            Thread.Sleep(500);

            //Assert
            var roomActor = Sys.ActorSelection(bridge.Path.Child(Actors.RoomBridgeActor.RoomActorName));
            roomActor.Tell("Hello");
            ExpectMsg<string>(msg => msg.ShouldBe("Hello"));
        }
    }
}
