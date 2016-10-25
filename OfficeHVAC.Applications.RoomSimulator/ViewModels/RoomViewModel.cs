using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using OfficeHVAC.Actors;
using Prism.Mvvm;

namespace OfficeHVAC.Applications.RoomSimulator.ViewModels
{
    public class RoomViewModel : BindableBase
    {
        public IActorRef RoomActor { get; set; }
        public Props RoomActorProps { get; set; }
        public IActorRef BridgeActor { get; set; }
        public Props BridgeActorProps { get; set; }
        
        private float temperature;
        public float Temperature
        {
            get { return temperature; }
            set { SetProperty(ref temperature, value); }
        }

        private string roomName;
        public string RoomName
        {
            get { return roomName; }
            set { SetProperty(ref roomName, value); }
        }

        public ConnectionConfig.Builder ConnectionConfigBuilder { get; set; } = new ConnectionConfig.Builder();

        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            private set { SetProperty(ref isConnected, value); }
        }

        public ActorSystem LocalActorSystem { get; set; }

        public void InitializeSimulator()
        {
            IsConnected = true;
            try
            {
                if (string.IsNullOrWhiteSpace(this.RoomName))
                    throw new ArgumentException();

                IConnectionConfig connectionConfig = this.ConnectionConfigBuilder.Build();

                this.LocalActorSystem = ActorSystem.Create("OfficeHVAC", connectionConfig.Configuration);
                this.BridgeActor = this.LocalActorSystem.ActorOf(this.BridgeActorProps, "bridge");
                this.RoomActor =   this.LocalActorSystem.ActorOf(this.RoomActorProps, this.RoomName);
            }
            catch (Exception)
            {
                LocalActorSystem?.Terminate();
                IsConnected = false;
                LocalActorSystem = null;
            }
        }

    }
}
