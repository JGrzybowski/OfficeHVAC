using Autofac;
using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using OfficeHVAC.Modules.TimeSimulation.Views;
using Prism.Regions;
using System;

namespace OfficeHVAC.Modules.TimeSimulation
{
    public class TimeSimulationModule : Module, Prism.Modularity.IModule
    {
        readonly IRegionManager _regionManager;

        public TimeSimulationModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("TimeSimulationRegion", typeof(TimeControl));
        }

        public static void InitializeDependencies (ContainerBuilder containerBuilder)
        {
            var now = Instant.FromDateTimeUtc(DateTime.UtcNow.AddDays(5).Date);

            containerBuilder.Register(context => now).As<Instant>();
            containerBuilder.RegisterType<ControlledTimeSource>()
                .As<ITimeSource>()
                .As<IControlledTimeSource>()
                .SingleInstance();
            containerBuilder.RegisterType<TimeControlViewModel>()
                .AsSelf()
                .As<ITimeControlViewModel>();
        }
    }
}
