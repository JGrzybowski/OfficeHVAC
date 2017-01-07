using Autofac;
using Autofac.Core;
using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Factories.Configs;
using OfficeHVAC.Factories.Simulators.Temperature;
using OfficeHVAC.Models;
using OfficeHVAC.Models.TimeSources;
using OfficeHVAC.Modules.RoomSimulator.Factories;
using OfficeHVAC.Modules.RoomSimulator.ViewModels;
using Prism.Regions;

namespace OfficeHVAC.Modules.RoomSimulator
{
    public class RoomSimulatorModule : Module, Prism.Modularity.IModule
    {
        readonly IRegionManager _regionManager;

        //TODO: 07.  Implement the module constructor to bring in required objects.
        //          When Prism loads the module it will instantiate this class using
        //          Autofac DI, Autofac will then inject a Region Manager instance.
        public RoomSimulatorModule(IRegionManager regionManager, ContainerBuilder containerBuilder)
        {
            _regionManager = regionManager;

            InitializeDependencies(containerBuilder);
        }

        //TODO: 08. Implement the required Initialize method to provide an entry point
        //         for your modules startup code.  Here we are registering ViewA with
        //         with the Autofac DI Container and also adding it to the "MainRegion"
        //         which was defined on the MainWindow in the HelloWorld project.
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(OfficeHVAC.Modules.RoomSimulator.Views.RoomSimulator));
        }

        public static void InitializeDependencies (ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<RealTimeSource>().As<ITimeSource>();
            containerBuilder.RegisterType<RoomSimulatorModule>();
            containerBuilder.RegisterType<RemoteConfigBuilder>().As<IConfigBuilder>();
            containerBuilder.RegisterType<RemoteActorPathBuilder>()
                .WithProperties(new Parameter[]
                {
                    new NamedPropertyParameter(nameof(RemoteActorPathBuilder.CompanyActorName), "Logger"),
                    new NamedPropertyParameter(nameof(RemoteActorPathBuilder.ServerAddress), "localhost"),
                    new NamedPropertyParameter(nameof(RemoteActorPathBuilder.ServerPort), 8000)
                })
                .As<IRemoteActorPathBuilder>()
                .As<IActorPathBuilder>();

            containerBuilder.RegisterType<TemperatureSimulatorFactory>().As<ITemperatureSimulatorFactory>();

            containerBuilder.RegisterType<RoomSimulatorActorPropsFactory>().As<IRoomSimulatorActorPropsFactory>();
            containerBuilder.RegisterType<BridgeRoomActorPropsFactory>().As<IBridgeRoomActorPropsFactory>();

            containerBuilder.RegisterType<RoomSimulatorViewModel>().AsSelf().As<IRoomViewModel>();

        }
    }
}
