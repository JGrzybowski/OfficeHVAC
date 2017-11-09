using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using OfficeHVAC.Models;
using Shouldly;
using System;
using OfficeHVAC.Models.Subscription;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Actors.RoomActor
{
    public class Subscriptions : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const double TemperatureInRoom = 20;

        private readonly IActorRef blackHole;

        public Subscriptions()
        {
            blackHole = ActorOf(BlackHoleActor.Props);
        }

        private Props RoomActorProps() =>
            Props.Create(() => new RoomSimulator.Actors.RoomActor(
                new RoomStatus()
                {
                    Name = TestRoomName,
                    Parameters = new ParameterValuesCollection()
                    {
                        new ParameterValue(SensorType.Temperature, TemperatureInRoom)
                    }
                },
                blackHole)
            );

        [Fact]
        public void sends_room_status_when_some_actor_subscribes()
        {
            //Arrange
            var actor = ActorOf(RoomActorProps());

            //Act
            actor.Tell(new SubscribeMessage(TestActor));

            //Assert
            ExpectMsg<IRoomStatusMessage>(msg =>
            {
                msg.Name.ShouldBe(TestRoomName);
                msg.Parameters[SensorType.Temperature].Value.ShouldBe(TemperatureInRoom);
            });
        }

        [Fact]
        public void sends_status_update_info_to_subscribes_when_recieving_update_request()
        {
            //Arrange
            var actor = ActorOf(RoomActorProps());
            actor.Tell(new SubscribeMessage(TestActor));
            ExpectMsg<IRoomStatusMessage>();

            //Act
            actor.Tell(new SubscriptionTriggerMessage());

            //Assert
            ExpectMsg<IRoomStatusMessage>(msg =>
            {
                msg.Name.ShouldBe(TestRoomName);
                msg.Parameters[SensorType.Temperature].Value.ShouldBe(TemperatureInRoom);
            },
            timeout: TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void does_not_send_status_update_info_to_unsubscribed_actors_when_recieving_update_request()
        {
            //Arrange
            var actor = this.ActorOfAsTestActorRef<RoomSimulator.Actors.RoomActor>(RoomActorProps());
            actor.Tell(new SubscribeMessage(TestActor));
            ExpectMsg<IRoomStatusMessage>();
            actor.Tell(new UnsubscribeMessage(TestActor));

            //Act
            actor.Tell(new SubscriptionTriggerMessage());

            //Assert
            ExpectNoMsg();
        }
    }
}
