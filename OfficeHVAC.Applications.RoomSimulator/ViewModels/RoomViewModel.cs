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

        public IRoomSimulatorActorPropsFactory RoomSimulatorActorPropsFactory { get; }

        public IConfigBuilder ConfigBuilder { get; }

        public RoomViewModel(IRoomSimulatorActorPropsFactory roomSimulatorActorPropsFactory, IConfigBuilder configBuilder)
        {
            this.RoomSimulatorActorPropsFactory = roomSimulatorActorPropsFactory;
            this.ConfigBuilder = configBuilder;
        }

        // Old Fields
        public IActorRef BridgeActor { get; private set; }
        public Props BridgeActorProps { get; set; }

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
                this.LocalActorSystem = ActorSystem.Create(this.ActorSystemName, actorSystemConfig);
                this.LocalActorSystem.ActorOf(this.RoomSimulatorActorPropsFactory.Props(), RoomActorName);
                this.BridgeActor = this.LocalActorSystem.ActorOf(this.BridgeActorProps, "bridge");
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
