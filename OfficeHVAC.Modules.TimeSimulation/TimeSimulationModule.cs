using Autofac;
using OfficeHVAC.Models;
using OfficeHVAC.Models.TimeSources;
using Prism.Regions;

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
            //_regionManager.RegisterViewWithRegion("MainRegion", typeof(OfficeHVAC.Modules.RoomSimulator));
        }

        public static void InitializeDependencies (ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<RealTimeSource>().As<ITimeSource>();
            //containerBuilder.RegisterType<ControlledTimeSource>().As<ITimeSource>();
        }
    }
}
