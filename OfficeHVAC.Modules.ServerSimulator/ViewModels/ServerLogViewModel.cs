using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Akka.Actor;
using Akka.Configuration;
using OfficeHVAC.Factories.Configs;
using Prism.Commands;
using Prism.Mvvm;

namespace OfficeHVAC.Modules.ServerSimulator.ViewModels
{
    public class ServerLogViewModel : BindableBase, IServerLogViewModel
    {
        //Constants 
        public const string BridgeActorName = "bridge";
        public string ActorSystemName { get; } = "OfficeHVAC";

        // Dependencies
        public IConfigBuilder ConfigBuilder { get; }

        // Actor System fields
        public IActorRef BridgeActor { get; private set; }
        public ActorSystem LocalActorSystem { get; set; }

        private bool _isConnected;
        private readonly object _lock = new object();

        public bool IsConnected
        {
            get { return _isConnected; }
            private set { SetProperty(ref _isConnected, value); }
        }

        public ICommand InitializeCommand { get; }

        public ServerLogViewModel(IConfigBuilder configBuilder)
        {
            this.ConfigBuilder = configBuilder;
            this.ConfigBuilder.Port = 8000;

            InitializeCommand = new DelegateCommand(InitializeSimulator, () => !IsConnected).ObservesProperty(() => IsConnected);
            BindingOperations.EnableCollectionSynchronization(Logs, _lock);
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
                Config actorSystemConfig = this.ConfigBuilder.Config();
                var logactorProps = Props.Create(() => new LoggerActor());
                var bridgeProps = Props.Create(() => new ServerBridgeActor(this, logactorProps));

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
