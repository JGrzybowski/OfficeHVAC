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
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.Actors
{
    public class RoomSimulatorActorTests : TestKit
    {
        private const string TestRoomName = "Room 101";
        private const float TemperatureInRoom = 20f;

        private class TemperatureSimulatorFake : ITemperatureSimulator
        {
            public double Temperature { get; set; }
            public IEnumerable<ITemperatureDevice> Devices { get; set; }
            public ITimeSource TimeSource { get; }
            public ITemperatureModel Model { get; }
        }

        private static ITemperatureSimulator GenerateTemperatureSimulatorFake()
        {
            var fake = Substitute.For<ITemperatureSimulator>();
            fake.Temperature.Returns(TemperatureInRoom);
            return fake;
        }

        private Props SimulatorActorProps(ActorPath companyActorPath) =>
            Props.Create(() => new RoomSimulatorActor(TestRoomName, GenerateTemperatureSimulatorFake(), companyActorPath));
        
        [Fact]
        public void sends_avaliability_message_to_server()
        {
            var simulatorProps = SimulatorActorProps(TestActor.Path);
            var actor = ActorOf(simulatorProps);
            ExpectMsg<RoomAvaliabilityMessage>(msg => msg.RoomActor.ShouldBe(actor));
        }

        [Theory]
        [InlineData(+1)]
        [InlineData(-1)]
        public void changes_temperature_in_simulator_accoding_to_message(float deltaT)
        {
            //Arrange
            var temperatureSimulatorFake = new TemperatureSimulatorFake();
            var roomActor = ActorOfAsTestActorRef(() => new RoomSimulatorActor(TestRoomName, temperatureSimulatorFake, ActorOf(BlackHoleActor.Props).Path));
            var temperatureBefore = temperatureSimulatorFake.Temperature;

            //Act
            roomActor.Tell(new ChangeTemperature(deltaT));

            //Assert
            Thread.Sleep(500);
            var temperatureAfter = temperatureSimulatorFake.Temperature;
            temperatureAfter.ShouldBe(temperatureBefore + deltaT);
        }

        [Fact]
        public void sends_initial_room_status_when_some_actor_subscribes()
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
        public void sends_status_update_info_to_subscribes_from_scheduler()
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
            timeout: TimeSpan.FromSeconds(1));
            ExpectMsg<RoomStatusMessage>(msg =>
            {
                msg.RoomName.ShouldBe(TestRoomName);
                msg.Temperature.ShouldBe(TemperatureInRoom);
            },
            timeout: TimeSpan.FromSeconds(2));
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
