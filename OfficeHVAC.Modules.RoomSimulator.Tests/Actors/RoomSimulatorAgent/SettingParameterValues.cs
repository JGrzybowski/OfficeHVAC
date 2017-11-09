using System;
using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.RoomSimulator.Actors;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Actors.RoomSimulatorAgent
{
    public class SettingParameterValues : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const float TemperatureInRoom = 20f;

        private readonly IActorRef blackHoleActorRef;

        public SettingParameterValues()
        {
            blackHoleActorRef = ActorOf(BlackHoleActor.Props);
        }
        
        private Props SimulatorActorProps() =>
            RoomSimulatorActor.Props(
                new RoomStatus()
                {
                    Name = TestRoomName,
                    Parameters = new ParameterValuesCollection()
                    {
                        new ParameterValue(SensorType.Temperature, TemperatureInRoom)
                    }
                },
                blackHoleActorRef.Path
            );

        [Theory]
        [InlineData(SensorType.Temperature, +1)]
        [InlineData(SensorType.Temperature, -1)]
        public async Task changes_known_parameter_Value_in_RoomStatus_accoding_to_the_message(SensorType paramType, object value)
        {
            //Arrange
            var roomActor = ActorOf(SimulatorActorProps());

            //Act
            roomActor.Tell(new ParameterValueMessage(paramType, value));

            //Assert
            var status = await roomActor.Ask<IRoomStatusMessage>(new RoomStatus.Request(), timeout: TimeSpan.FromSeconds(5));
            status.Parameters[SensorType.Temperature].Value.ShouldBe(value);
        }
    }
}
