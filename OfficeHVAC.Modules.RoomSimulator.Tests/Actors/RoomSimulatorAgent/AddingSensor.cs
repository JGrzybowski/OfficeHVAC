﻿using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Actors.RoomSimulatorAgent
{
    public class AddingSensor : TestKit
    {
        private const string TestRoomName = "Room 101";

        private Props RoomActorProps() =>
            Props.Create(() => new RoomSimulator.Actors.RoomSimulatorActor(
                new RoomStatus()
                {
                    Name = TestRoomName,
                    Parameters = new ParameterValuesCollection()
                },
                ActorOf(BlackHoleActor.Props).Path                
            ));
        
        [Fact]
        public void SendsStatusToIt()
        {
            //Arrange
            var actor = ActorOf(RoomActorProps());
            IgnoreMessages<SubscribeMessage>();
            
            //Act
            actor.Tell(new SensorAvaliableMessage(SensorType.Temperature, "Sendor ID"));
            
            //Assert
            ExpectMsg<IRoomStatusMessage>();
        }
    }
}