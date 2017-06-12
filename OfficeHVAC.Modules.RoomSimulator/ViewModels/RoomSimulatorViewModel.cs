using Akka.Actor;
using OfficeHVAC.Modules.RoomSimulator.Factories;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OfficeHVAC.Modules.RoomSimulator.ViewModels
{
    public class RoomSimulatorViewModel : BindableBase, IRoomViewModel
    {
        //Constants 
        public const string BridgeActorNamePostxix = "bridge";

        // Dependencies
        public IBridgeRoomActorPropsFactory BridgeRoomActorPropsFactory { get; }

        // Actor System fields
        public IActorRef BridgeActor { get; private set; }
        public ActorSystem LocalActorSystem { get; set; }

        // Notifiable Properties 
        private double _temperature;
        public double Temperature
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

        public ICommand InitializeCommand { get; }
        // Constructor
        public RoomSimulatorViewModel(IBridgeRoomActorPropsFactory bridgeRoomActorPropsFactory, ActorSystem actorSystem)
        {
            this.LocalActorSystem = actorSystem;

            this.BridgeRoomActorPropsFactory = bridgeRoomActorPropsFactory;
            this.BridgeRoomActorPropsFactory.ViewModel = this;
            this.BridgeRoomActorPropsFactory.RoomSimulatorActorPropsFactory.RoomName = "Room101";

            //IncreaseCommand = new DelegateCommand(
            //        () => BridgeActor.Tell(new ChangeTemperature(1)),
            //        () => this.IsConnected)
            //    .ObservesProperty(() => IsConnected);

            //DecreaseCommand = new DelegateCommand(
            //        () => BridgeActor.Tell(new ChangeTemperature(-1)),
            //        () => this.IsConnected)
            //    .ObservesProperty(() => IsConnected);

            InitializeCommand =
                new DelegateCommand(InitializeSimulator, () => !IsConnected).ObservesProperty(() => IsConnected);
        }

        public void InitializeSimulator()
        {
            IsConnected = true;
            try
            {
                var bridgeProps = this.BridgeRoomActorPropsFactory.Props();
                var bridgeActorName = $"{BridgeRoomActorPropsFactory.RoomSimulatorActorPropsFactory.RoomName}-{BridgeActorNamePostxix}";
                this.BridgeActor = this.LocalActorSystem.ActorOf(bridgeProps, bridgeActorName);
            }
            catch (Exception)
            {
                this.IsConnected = false;
                this.LocalActorSystem = null;
            }
        }

        public async Task ShutdownSimulator()
        {
            BridgeActor.Tell(PoisonPill.Instance);
            this.BridgeActor = null;
            this.IsConnected = false;
        }
    }
}
