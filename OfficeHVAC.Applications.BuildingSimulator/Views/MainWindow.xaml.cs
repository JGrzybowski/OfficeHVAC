using Syncfusion.Windows.Tools.Controls;
using System.Windows;
using OfficeHVAC.Applications.BuildingSimulator.ViewModels;

namespace OfficeHVAC.Applications.BuildingSimulator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            PropertiesEditor.HidePropertiesCollection.Add(nameof(ITreeElement.SubItems));
        }

        //private void treeviewitem_AfterItemEdit(object sender, EditModeChangeEventArgs e)
        //{
        //    (e.Source as ITreeElement).Name = e.NewValue.ToString();
        //}

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel dc)
                dc.IsSimulatorRunning = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel dc)
                dc.IsSimulatorRunning = false;
        }
        
        private void SeedDataButton_Click(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as MainWindowViewModel;
            dc?.SeedData();
        }
    }
}