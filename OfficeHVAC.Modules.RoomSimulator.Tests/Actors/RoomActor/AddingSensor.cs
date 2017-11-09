using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Actors.RoomActor
{
    public class AddingSensor : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const float TemperatureInRoom = 20f;

        private Props RoomActorProps() =>
            Props.Create(() => new RoomSimulator.Actors.RoomActor(
                new RoomStatus()
                {
                    Name = TestRoomName,
                    Parameters = new ParameterValuesCollection()
                },
                ActorOf(BlackHoleActor.Props)
            ));
        
        [Fact]
        public void SendsSubscrbeMessageToIt()
        {
            //Arrange
            var actor = ActorOf(RoomActorProps());
            
            //Act
            actor.Tell(new SensorAvaliableMessage(SensorType.Temperature, "Sendor ID"));
            
            //Assert
            ExpectMsg<SubscribeMessage>(msg => msg.Subscriber.Path.ShouldBe(actor.Path));
        }
    }
}