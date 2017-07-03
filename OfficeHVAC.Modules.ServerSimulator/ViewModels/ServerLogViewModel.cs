using Akka.Actor;
using Akka.TestKit;
using OfficeHVAC.Models;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace OfficeHVAC.Modules.ServerSimulator.ViewModels
{
    public class ServerLogViewModel : BindableBase, IServerLogViewModel
    {
        //Constants 
        public const string BridgeActorName = "bridge";

        // Dependencies
        public ActorSystem LocalActorSystem { get; set; }

        public ITimeSource TimeSource;

        // Actor System fields
        public IActorRef BridgeActor { get; private set; }

        private bool _isConnected;
        private readonly object _lock = new object();

        public bool IsConnected
        {
            get { return _isConnected; }
            private set { SetProperty(ref _isConnected, value); }
        }

        public ICommand InitializeCommand { get; }

        public ServerLogViewModel(ActorSystem actorSystem, TestScheduler scheduler, ITimeSource timeSource)
        {
            this.LocalActorSystem = actorSystem;
            this.TimeSource = timeSource;
            BindingOperations.EnableCollectionSynchronization(Logs, _lock);
            InitializeSimulator();
        }

        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        public void Log(string message)
        {
            Logs.Add(message);
        }

        public void InitializeSimulator()
        {
            IsConnected = true;
            try
            {
                var logactorProps = Props.Create(() => new LoggerActor(this.TimeSource));
                var bridgeProps = Props.Create(() => new ServerBridgeActor(this, logactorProps));
                this.BridgeActor = this.LocalActorSystem.ActorOf(bridgeProps, BridgeActorName);
            }
            catch (Exception)
            {
                // TODO Log exception
                this.IsConnected = false;
            }
        }

        public async Task ShutdownSimulator()
        {
            this.BridgeActor = null;
            this.IsConnected = false;
        }
    }
}
