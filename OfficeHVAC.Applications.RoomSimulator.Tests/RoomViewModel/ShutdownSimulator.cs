﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Actors;
using OfficeHVAC.Messages;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomViewModel
{
    public class ShutdownSimulator :TestKit
    {
        private IActorRef blackHole;
        private IActorRef echoActor;
        private ConnectionConfig.Builder connectionConfigFake;

        public ShutdownSimulator()
        {
            blackHole = ActorOf(BlackHoleActor.Props);
            echoActor = ActorOf(EchoActor.Props(this, false));
        }

        private void SetupFakes(Config hostingConfig, ActorPath companyActorPath)
        {
            var configStub = new ConnectionConfig(hostingConfig, companyActorPath);
            connectionConfigFake = Substitute.For<ConnectionConfig.Builder>();
            connectionConfigFake.Build().Returns(configStub);
        }

        [Fact]
        public async Task cleans_up_viewModel()
        {
            //Arrange
            SetupFakes(Config.Empty, TestActor.Path);
            var vm = new ViewModels.RoomViewModel
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
                RoomActorProps = BlackHoleActor.Props,
                RoomName = "room"
            };
            vm.InitializeSimulator();
            vm.IsConnected.ShouldBe(true);

            //Act
            await vm.ShutdownSimulator();

            //Assert
            vm.IsConnected.ShouldBe(false);
            vm.RoomActor.ShouldBeNull();
            vm.BridgeActor.ShouldBeNull();
            vm.LocalActorSystem.ShouldBeNull();
        }
    }
}
