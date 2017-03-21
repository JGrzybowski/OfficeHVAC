﻿using Akka.Actor;
using Akka.TestKit.Xunit2;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Actors.RoomActor
{
    public class Avaliability : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const float TemperatureInRoom = 20f;

        private Props RoomActorProps(ActorPath companyActorPath) =>
            Props.Create(() => new RoomSimulator.Actors.RoomActor(
                new RoomInfo() { Name = TestRoomName },
                companyActorPath,
                new ParameterValuesCollection())
            );

        [Fact]
        public void sends_avaliability_message_to_server()
        {
            var roomActorProps = RoomActorProps(TestActor.Path);
            var actor = ActorOf(roomActorProps);
            ExpectMsg<RoomAvaliabilityMessage>(msg => msg.RoomActor.ShouldBe(actor));
        }
    }
}
