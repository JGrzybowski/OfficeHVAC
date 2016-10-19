using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Messages;
using OfficeHVAC.Simulators;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Actors.Tests
{
    public class RoomSimulatorActor : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const float TemperatureInRoom = 20f;

        private ITemperatureSimulator GenerateTemperatureSimulatorFake()
        {
            var fake = Substitute.For<ITemperatureSimulator>();
            fake.Temperature.Returns(TemperatureInRoom);
            return fake;
        }

        [Fact]
        public void sends_initial_room_info_when_some_actor_subscribes()
        {
            //Arrange
            var actor = ActorOfAsTestActorRef(() => 
                new Actors.RoomSimulatorActor(TestRoomName, GenerateTemperatureSimulatorFake())
            );
            
            //Act
            actor.Tell(new SubscribeMessage(actor));

            //Assert
            ExpectMsg<RoomStatusMessage>((msg, sender) =>
            {
                msg.RoomName.ShouldBe(TestRoomName);
                msg.Temperature.ShouldBe(TemperatureInRoom);
            });

        }

        [Fact]
        public void sends_status_update_info_to_subscribes_when_recieving_update_request()
        {
            //Arrange
            var actor = ActorOfAsTestActorRef(() =>
                new Actors.RoomSimulatorActor(TestRoomName, GenerateTemperatureSimulatorFake())
            );
            actor.Tell(new SubscribeMessage(actor));
            ExpectMsg<RoomStatusMessage>();
            
            //Act
            actor.Tell(new RoomStatusRequest());

            //Assert
            ExpectMsg<RoomStatusMessage>((msg, sender) =>
            {
                msg.RoomName.ShouldBe(TestRoomName);
                msg.Temperature.ShouldBe(TemperatureInRoom);
            });
        }

        [Fact]
        public void does_not_send_status_update_info_to_unsubscribed_actors_when_recieving_update_request()
        {
            //Arrange
            var actor = ActorOfAsTestActorRef(() =>
                new Actors.RoomSimulatorActor(TestRoomName, GenerateTemperatureSimulatorFake())
            );

            actor.Tell(new SubscribeMessage(actor));
            ExpectMsg<RoomStatusMessage>();
            actor.Tell(new UnsubscribeMessage(actor));

            //Act
            actor.Tell(new RoomStatusRequest());

            //Assert
            ExpectNoMsg();
        }
    }
}
