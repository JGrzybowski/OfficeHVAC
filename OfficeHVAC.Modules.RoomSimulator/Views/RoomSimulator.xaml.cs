using System.Windows.Controls;
using OfficeHVAC.Modules.RoomSimulator.ViewModels;

namespace OfficeHVAC.Modules.RoomSimulator.Views
{
    /// <summary>
    /// Interaction logic for RoomSimulatorControl.xaml
    /// </summary>
    public partial class RoomSimulator : UserControl
    {
        private RoomSimulatorViewModel  ViewModel => this.DataContext as RoomSimulatorViewModel;

        public RoomSimulator()
        {
            InitializeComponent();
        }

        private void InitializationButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.InitializeSimulator();
        }
    }
}
