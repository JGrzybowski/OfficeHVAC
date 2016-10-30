﻿using Akka.Actor;
using Akka.Configuration;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.Configs;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;

namespace OfficeHVAC.Applications.RoomSimulator.ViewModels
{
    public class RoomViewModel : BindableBase, IRoomViewModel
    {
        //Constants 
        public const string BridgeActorName = "bridge";
        public string ActorSystemName { get; } = "OfficeHVAC";

        // Dependencies
        public IConfigBuilder ConfigBuilder { get; }
        public IBridgeRoomActorPropsFactory BridgeRoomActorPropsFactory { get; }

        // Actor System fields
        public IActorRef BridgeActor { get; private set; }
        public ActorSystem LocalActorSystem { get; set; }

        // Notifiable Properties 
        private float _temperature;
        public float Temperature
        {
            get { return _temperature; }
            set { SetProperty(ref _temperature, value); }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            private set { SetProperty(ref _isConnected, value); }
        }     

        // Constructor
        public RoomViewModel(IConfigBuilder configBuilder, IBridgeRoomActorPropsFactory bridgeRoomActorPropsFactory)
        {
            this.ConfigBuilder = configBuilder;
            this.BridgeRoomActorPropsFactory = bridgeRoomActorPropsFactory;
        }

        public void InitializeSimulator()
        {
            IsConnected = true;
            try
            {
                Config actorSystemConfig = this.ConfigBuilder.Config();
                var bridgeProps = this.BridgeRoomActorPropsFactory.Props();

                this.LocalActorSystem = ActorSystem.Create(this.ActorSystemName, actorSystemConfig);
                this.BridgeActor = this.LocalActorSystem.ActorOf(bridgeProps, BridgeActorName);
            }
            catch (Exception)
            {
                // TODO Log exception
                this.LocalActorSystem?.Terminate();
                this.IsConnected = false;
                this.LocalActorSystem = null;
            }
        }

        public async Task ShutdownSimulator()
        {
            this.BridgeActor = null;
            await this.LocalActorSystem.Terminate();
            this.LocalActorSystem = null;
            this.IsConnected = false;
        }
    }
}
