using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit;
using Autofac;
using OfficeHVAC.Applications.BuildingSimulator.ViewModels;
using OfficeHVAC.Modules.TimeSimulation;
using Prism.Autofac;
using System.Windows;

namespace OfficeHVAC.Applications.BuildingSimulator
{
    class Bootstrapper : AutofacBootstrapper
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            builder.RegisterInstance
                (
                    ActorSystem.Create("OfficeHVAC", 
                                        ConfigurationFactory.ParseString(@"akka.scheduler.implementation = ""Akka.TestKit.TestScheduler, Akka.TestKit"""))
                )
                .As<ActorSystem>();

            builder.Register(ctx =>
                {
                    var scheduler = ctx.Resolve<ActorSystem>().Scheduler as TestScheduler;
                    return scheduler;
                })
                .As<TestScheduler>()
                .SingleInstance();

            builder.RegisterType<BuildingViewModel>().AsSelf();

            TimeSimulationModule.InitializeDependencies(builder);

            base.ConfigureContainerBuilder(builder);
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Views.MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
