using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.Configs;
using OfficeHVAC.Messages;
using Shouldly;
using System.Threading;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomBridgeActor
{
    public class Responses :TestKit
    {

        private readonly ViewModels.RoomSimulatorViewModel _viewModel = 
            Substitute.For<ViewModels.RoomSimulatorViewModel>(
                Substitute.For<IConfigBuilder>(),
                Substitute.For<IBridgeRoomActorPropsFactory>()
            );

        private readonly Props _echoActorProps;

        public Responses()
        {
            _echoActorProps = EchoActor.Props(this, false);
        }

        [Fact]
        public void forwards_SetTemperature_message_to_RoomActor()
        {
            //Arrange
            var bridge = ActorOf(() => new Actors.RoomBridgeActor(_viewModel,_echoActorProps), "bridge");
            ExpectMsg<SubscribeMessage>();
            var roomActor = Sys.ActorSelection(bridge, "room").Anchor;

            //Act
            bridge.Tell(new SetTemperature(30f));

            //Assert
            ExpectMsg<SetTemperature>((msg, sender) =>
            {
                msg.Temperature.ShouldBe(30f); 
                sender.ShouldBe(roomActor);
            });
        }

        [Theory]
        [InlineData(+2.5f)]
        [InlineData(-2.2f)]
        public void forwards_ChangeTemperature_message_to_RoomActor(float deltaT)
        {
            //Arrange
            var bridge = ActorOf(() => new Actors.RoomBridgeActor(_viewModel, _echoActorProps), "bridge");
            ExpectMsg<SubscribeMessage>();
            var roomActor = Sys.ActorSelection(bridge, "room").Anchor;

            //Act
            bridge.Tell(new ChangeTemperature(deltaT));

            //Assert
            ExpectMsg<ChangeTemperature>((msg, sender) =>
            {
                msg.DeltaT.ShouldBe(deltaT);
                sender.ShouldBe(roomActor);
            });
        }

        [Fact]
        public void updates_ViewModel_Temperature_when_recieved_RoomStatusMessage()
        {
            //Arrange
            var bridge = ActorOf(() => new Actors.RoomBridgeActor(_viewModel, _echoActorProps), "bridge");
            ExpectMsg<SubscribeMessage>();

            //Act
            bridge.Tell(new RoomStatusMessage("Room 101", 26f));

            //Assert
            Thread.Sleep(1000);
            _viewModel.Received().Temperature = 26f;
        }
    }
}
