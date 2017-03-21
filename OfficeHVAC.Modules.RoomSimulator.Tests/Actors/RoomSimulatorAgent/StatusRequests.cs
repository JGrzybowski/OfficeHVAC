using System;
using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using OfficeHVAC.Models;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Actors.RoomSimulatorAgent
{
    public class StatusRequests : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const float TemperatureInRoom = 20f;

        private Props RoomActorProps() =>
            Props.Create(() => new RoomSimulator.Actors.RoomSimulatorActor(
                TestRoomName,
                ActorOf(BlackHoleActor.Props).Path,
                new ParameterValuesCollection() { new ParameterValue(SensorType.Temperature, TemperatureInRoom) })
            );

        [Fact]
        public void responds_with_room_status_when_requested()
        {
            //Arrange
            var actor = ActorOf(RoomActorProps());

            //Act
            actor.Tell(new RoomStatus.Request());

            //Assert
            ExpectMsg<IRoomStatusMessage>(msg =>
            {
                msg.RoomInfo.Name.ShouldBe(TestRoomName);
                msg.Parameters[SensorType.Temperature].Value.ShouldBe(TemperatureInRoom);
            });
        }
    }
}
