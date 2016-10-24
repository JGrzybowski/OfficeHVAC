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
        public Props RoomActorProps { get; set; }
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
            set { SetProperty(ref isConnected, value); }
        }

        public ActorSystem ActorSystem { get; set; }

        public void InitializeSimulator()
        {
            throw new NotImplementedException();
        }
    }
}
