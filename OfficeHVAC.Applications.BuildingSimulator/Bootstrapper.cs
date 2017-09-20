using Autofac;
using Prism.Autofac;
using System.Windows;

namespace OfficeHVAC.Applications.BuildingSimulator
{
    class Bootstrapper : AutofacBootstrapper
    {
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
