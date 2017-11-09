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
        private double temperature;
        public double Temperature
        {
            get => temperature;
            set => SetProperty(ref temperature, value);
        }

        private bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            private set => SetProperty(ref isConnected, value);
        }

        public ICommand InitializeCommand { get; }
        
        // Constructor
        public RoomSimulatorViewModel(IBridgeRoomActorPropsFactory bridgeRoomActorPropsFactory, ActorSystem actorSystem)
        {
            LocalActorSystem = actorSystem;

            BridgeRoomActorPropsFactory = bridgeRoomActorPropsFactory;
            BridgeRoomActorPropsFactory.ViewModel = this;
            BridgeRoomActorPropsFactory.RoomSimulatorActorPropsFactory.RoomName = "Room101";

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
                var bridgeProps = BridgeRoomActorPropsFactory.Props();
                var bridgeActorName = $"{BridgeRoomActorPropsFactory.RoomSimulatorActorPropsFactory.RoomName}-{BridgeActorNamePostxix}";
                BridgeActor = LocalActorSystem.ActorOf(bridgeProps, bridgeActorName);
            }
            catch (Exception)
            {
                IsConnected = false;
                LocalActorSystem = null;
            }
        }

        public async Task ShutdownSimulator()
        {
            BridgeActor.Tell(PoisonPill.Instance);
            BridgeActor = null;
            IsConnected = false;
        }
    }
}
