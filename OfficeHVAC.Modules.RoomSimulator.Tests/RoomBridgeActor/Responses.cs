﻿using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using Shouldly;
using System.Threading;
using OfficeHVAC.Models.Subscription;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.RoomBridgeActor
{
    public class Responses : TestKit
    {
        private readonly ViewModels.IRoomViewModel _viewModel = Substitute.For<ViewModels.IRoomViewModel>();

        private readonly Props _echoActorProps;

        public Responses()
        {
            _echoActorProps = EchoActor.Props(this, false);
        }

        [Fact]
        public void forwards_SetTemperature_message_to_RoomActor()
        {
            //Arrange
            var bridge = ActorOf(() => new RoomSimulator.Actors.RoomBridgeActor(_viewModel, _echoActorProps), "bridge");
            ExpectMsg<SubscribeMessage>();
            var roomActor = Sys.ActorSelection(bridge, "room").Anchor;

            //Act
            bridge.Tell(new SetTemperature(30f));

            //Assert
            ExpectMsg<SetTemperature>((msg, sender) =>
            {
                msg.Temperature.ShouldBe(30f);
                sender.ShouldBe(roomActor);
            });
        }

        [Theory]
        [InlineData(+2.5f)]
        [InlineData(-2.2f)]
        public void forwards_ChangeTemperature_message_to_RoomActor(float deltaT)
        {
            //Arrange
            var bridge = ActorOf(() => new RoomSimulator.Actors.RoomBridgeActor(_viewModel, _echoActorProps), "bridge");
            ExpectMsg<SubscribeMessage>();
            var roomActor = Sys.ActorSelection(bridge, "room").Anchor;

            //Act
            bridge.Tell(new ChangeTemperature(deltaT));

            //Assert
            ExpectMsg<ChangeTemperature>((msg, sender) =>
            {
                msg.DeltaT.ShouldBe(deltaT);
                sender.ShouldBe(roomActor);
            });
        }

        [Fact]
        public void updates_ViewModel_Temperature_when_recieved_RoomStatusMessage_with_temperature()
        {
            //Arrange
            var bridge = ActorOf(() => new RoomSimulator.Actors.RoomBridgeActor(_viewModel, BlackHoleActor.Props), "bridge");

            //Act
            bridge.Tell(GenerateRoomStatusMessage("Room 101", 26));

            //Assert
            Thread.Sleep(1000);
            _viewModel.Temperature.ShouldBe(26);
        }

        private IRoomStatusMessage GenerateRoomStatusMessage(string roomName, double temperature) =>
            new RoomStatus()
            {
                Name = roomName,
                Parameters = new ParameterValuesCollection
                {
                    new ParameterValue(SensorType.Temperature, temperature)
                }
            };
    }
}
