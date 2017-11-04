﻿using System;
using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NodaTime;
using NSubstitute;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.RoomSimulator.Actors;
using OfficeHVAC.Modules.TemperatureSimulation;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Actors.RoomSimulatorAgent
{
    public class SettingParameterValues : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const float TemperatureInRoom = 20f;

        private static ITemperatureSimulatorFactory GenerateTemperatureSimulatorFake()
        {
            var simulatorFake = Substitute.For<ITemperatureSimulator>();
            simulatorFake.ChangeTemperature(Arg.Any<IRoomStatusMessage>(), Arg.Any<Duration>())
                         .Returns(TemperatureInRoom);

            var factoryFake = Substitute.For<ITemperatureSimulatorFactory>();
            factoryFake.TemperatureSimulator().Returns(simulatorFake);

            return factoryFake;
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
                ActorOf(BlackHoleActor.Props).Path
            );

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
            var status = await roomActor.Ask<IRoomStatusMessage>(new RoomStatus.Request(), timeout: TimeSpan.FromSeconds(5));
            status.Parameters[SensorType.Temperature].Value.ShouldBe(value);
        }
    }
}
