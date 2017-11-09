﻿using Autofac;
using OfficeHVAC.Modules.ServerSimulator.ViewModels;
using Prism.Regions;

namespace OfficeHVAC.Modules.ServerSimulator
{
    public class ServerSimulatorModule : Module, Prism.Modularity.IModule
    {
        readonly IRegionManager regionManager;

        //TODO: 07.  Implement the module constructor to bring in required objects.
        //          When Prism loads the module it will instantiate this class using
        //          Autofac DI, Autofac will then inject a Region Manager instance.
        public ServerSimulatorModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        //TODO: 08. Implement the required Initialize method to provide an entry point
        //         for your modules startup code.  Here we are registering ViewA with
        //         with the Autofac DI Container and also adding it to the "MainRegion"
        //         which was defined on the MainWindow in the HelloWorld project.
        public void Initialize()
        {
            regionManager.RegisterViewWithRegion("ServerLogRegion", typeof(Views.ServerLog));
        }

        public static void InitializeDependencies(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ServerLogViewModel>().AsSelf().As<IServerLogViewModel>();
        }
    }
}
