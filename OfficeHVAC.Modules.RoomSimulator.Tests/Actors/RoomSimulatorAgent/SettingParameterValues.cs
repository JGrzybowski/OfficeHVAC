using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Modules.RoomSimulator.Actors;
using OfficeHVAC.Modules.TemperatureSimulation;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Actors.RoomSimulatorAgent
{
    public class SettingParameterValues : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const float TemperatureInRoom = 20f;

        private class TemperatureSimulatorFake : ITemperatureSimulator
        {
            public double Temperature { get; set; }
            public IEnumerable<ITemperatureDevice> Devices { get; set; }
            public ITimeSource TimeSource { get; }
            public ITemperatureModel Model { get; }
            public double RoomVolume { get; set; }
        }

        private static ITemperatureSimulator GenerateTemperatureSimulatorFake()
        {
            var fake = Substitute.For<ITemperatureSimulator>();
            fake.Temperature.Returns(TemperatureInRoom);
            return fake;
        }

        private Props SimulatorActorProps() =>
            Props.Create(() => new RoomSimulatorActor(
                TestRoomName,
                ActorOf(BlackHoleActor.Props).Path,
                new ParameterValuesCollection() { new ParameterValue(SensorType.Temperature, TemperatureInRoom) }
            ));

        [Theory]
        [InlineData(SensorType.Temperature, +1)]
        [InlineData(SensorType.Temperature, -1)]
        public async Task changes_known_parameter_Value_in_RoomStatus_accoding_to_the_message(SensorType paramType, object value)
        {
            //Arrange
            var roomActor = ActorOfAsTestActorRef<RoomSimulatorActor>(SimulatorActorProps());

            //Act
            roomActor.Tell(new ParameterValueMessage(paramType, value));

            //Assert
            var status = await roomActor.Ask<IRoomStatusMessage>(new RoomStatus.Request());
            status.Parameters[SensorType.Temperature].Value.ShouldBe(value);
        }
    }
}
