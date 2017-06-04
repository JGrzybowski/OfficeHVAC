using Autofac;
using OfficeHVAC.Modules.RoomSimulator;
using OfficeHVAC.Modules.TimeSimulation;
using Prism.Autofac;
using Prism.Modularity;
using System.Windows;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit;
using OfficeHVAC.Modules.ServerSimulator;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;

namespace OfficeHVAC.Applications.RoomSimulator
{
    //TODO: 01. Create a Bootstrapper Class using AutofacBootstrapper
    public class Bootstrapper : AutofacBootstrapper
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder containerBuilder)
        {
            base.ConfigureContainerBuilder(containerBuilder);

            TimeSimulationModule.InitializeDependencies(containerBuilder);

            var actorSystem = ActorSystem.Create(
                "OfficeHVAC"
                ,ConfigurationFactory.ParseString(@"akka.scheduler.implementation = ""Akka.TestKit.TestScheduler, Akka.TestKit""")
            );

            containerBuilder.RegisterInstance(actorSystem)
                .As<ActorSystem>()
                .SingleInstance();

            containerBuilder.Register(ctx =>
            {
                var scheduler = ctx.Resolve<ActorSystem>().Scheduler as TestScheduler;
                return scheduler;
            })
            .As<TestScheduler>()
            .SingleInstance();

            ServerSimulatorModule.InitializeDependencies(containerBuilder);
            RoomSimulatorModule.InitializeDependencies(containerBuilder);
        }

        //TODO: 02. Override the CreateShell returning an instance of your shell.
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        //TODO: 03. Override the InitializeShell setting the MainWindow on the application and showing the shell.
        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        //TODO: 04. Override the ConfigureModuleCatalog 
        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog catalog = (ModuleCatalog)ModuleCatalog;
            catalog.AddModule(typeof(TimeSimulationModule));
            catalog.AddModule(typeof(RoomSimulatorModule));
            catalog.AddModule(typeof(ServerSimulatorModule));
        }
    }
}
