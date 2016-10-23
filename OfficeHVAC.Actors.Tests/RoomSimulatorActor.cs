using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Messages;
using OfficeHVAC.Simulators;
using Shouldly;
using System;
using Xunit;

namespace OfficeHVAC.Actors.Tests
{
    public class RoomSimulatorActor : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const float TemperatureInRoom = 20f;

        private static ITemperatureSimulator GenerateTemperatureSimulatorFake()
        {
            var fake = Substitute.For<ITemperatureSimulator>();
            fake.Temperature.Returns(TemperatureInRoom);
            return fake;
        }

        private Props SimulatorActorProps(ActorPath companyActorPath) =>
            Props.Create(() => new Actors.RoomSimulatorActor(TestRoomName, GenerateTemperatureSimulatorFake(), companyActorPath));



        [Fact]
        public void sends_avaliability_message_to_server()
        {
            var simulatorProps = SimulatorActorProps(this.TestActor.Path);
            var actor = ActorOf(simulatorProps);
            ExpectMsg<RoomAvaliabilityMessage>(msg => msg.RoomActor.ShouldBe(actor));
        }

        [Fact]
        public void sends_initial_room_info_when_some_actor_subscribes()
        {
            //Arrange
            var simulatorProps = SimulatorActorProps(ActorOf(BlackHoleActor.Props).Path);
            var actor = ActorOf(simulatorProps);
            
            //Act
            actor.Tell(new SubscribeMessage(TestActor));

            //Assert
            ExpectMsg<RoomStatusMessage>(msg =>
            {
                msg.RoomName.ShouldBe(TestRoomName);
                msg.Temperature.ShouldBe(TemperatureInRoom);
            });
        }

        [Fact]
        public void sends_status_update_info_to_subscribes_when_recieving_update_request()
        {
            //Arrange
            var simulatorProps = SimulatorActorProps(ActorOf(BlackHoleActor.Props).Path);
            var actor = ActorOf(simulatorProps);
            actor.Tell(new SubscribeMessage(TestActor));
            ExpectMsg<RoomStatusMessage>();
            
            //Act
            actor.Tell(new RoomStatusRequest());

            //Assert
            ExpectMsg<RoomStatusMessage>(msg =>
            {
                msg.RoomName.ShouldBe(TestRoomName);
                msg.Temperature.ShouldBe(TemperatureInRoom);
            },
            timeout : TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void does_not_send_status_update_info_to_unsubscribed_actors_when_recieving_update_request()
        {
            //Arrange
            var simulatorProps = SimulatorActorProps(ActorOf(BlackHoleActor.Props).Path);
            var actor = ActorOf(simulatorProps);
            actor.Tell(new SubscribeMessage(TestActor));
            ExpectMsg<RoomStatusMessage>();
            actor.Tell(new UnsubscribeMessage(TestActor));

            //Act
            actor.Tell(new RoomStatusRequest());

            //Assert
            ExpectNoMsg();
        }
    }
}
