using Akka.Actor;
using OfficeHVAC.Actors;
using OfficeHVAC.Factories.Propses;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using OfficeHVAC.Applications.RoomSimulator.Factories;

namespace OfficeHVAC.Applications.RoomSimulator.ViewModels
{
    public class RoomViewModel : BindableBase
    {
        // New fields 
        public const string RoomActorName = "room";
        public IRoomSimulatorActorPropsFactory RoomSimulatorActorPropsFactory { get; }

        public RoomViewModel(IRoomSimulatorActorPropsFactory roomSimulatorActorPropsFactory)
        {
            this.RoomSimulatorActorPropsFactory = roomSimulatorActorPropsFactory;
        }

        // Old Fields
        public IActorRef BridgeActor { get; private set; }
        public Props BridgeActorProps { get; set; }

        public ConnectionConfig.Builder ConnectionConfigBuilder { get; set; } = new ConnectionConfig.Builder();

        public string ActorSystemName { get; set; } = "OfficeHVAC";

        public ActorSystem LocalActorSystem { get; set; }
        
        private float _temperature;
        public float Temperature
        {
            get { return _temperature; }
            set { SetProperty(ref _temperature, value); }
        }

        private string _roomName;
        public string RoomName
        {
            get { return _roomName; }
            set { SetProperty(ref _roomName, value); }
        }
        
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
                IConnectionConfig connectionConfig = this.ConnectionConfigBuilder.Build();
                this.LocalActorSystem = ActorSystem.Create(this.ActorSystemName, connectionConfig.Configuration);
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
