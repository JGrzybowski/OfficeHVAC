using Akka.Actor;
using Akka.Configuration;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.Configs;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;

namespace OfficeHVAC.Applications.RoomSimulator.ViewModels
{
    public class RoomViewModel : BindableBase
    {
        // New fields 
        public const string RoomActorName = "room";

        public IConfigBuilder ConfigBuilder { get; }

        private float _temperature;
        public float Temperature
        {
            get { return _temperature; }
            set { SetProperty(ref _temperature, value); }
        }
        public IBridgeRoomActorPropsFactory BridgeRoomActorPropsFactory { get; }
        public IActorRef BridgeActor { get; private set; }

        public RoomViewModel(IConfigBuilder configBuilder, IBridgeRoomActorPropsFactory bridgeRoomActorPropsFactory)
        {
            this.ConfigBuilder = configBuilder;
            this.BridgeRoomActorPropsFactory = bridgeRoomActorPropsFactory;
        }

        // Old Fields
        public string ActorSystemName { get; } = "OfficeHVAC";

        public ActorSystem LocalActorSystem { get; set; }
        
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            private set { SetProperty(ref _isConnected, value); }
        }     

        public void InitializeSimulator()
        {
            IsConnected = true;
            try
            {
                Config actorSystemConfig = this.ConfigBuilder.Config();
                var bridgeProps = this.BridgeRoomActorPropsFactory.Props();

                this.LocalActorSystem = ActorSystem.Create(this.ActorSystemName, actorSystemConfig);
                this.BridgeActor = this.LocalActorSystem.ActorOf(bridgeProps, "bridge");
            }
            catch (Exception)
            {
                // TODO Log exception
                LocalActorSystem?.Terminate();
                IsConnected = false;
                LocalActorSystem = null;
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
