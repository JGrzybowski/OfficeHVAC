using System.Windows.Controls;
using OfficeHVAC.Modules.ServerSimulator.ViewModels;

namespace OfficeHVAC.Modules.ServerSimulator.Views
{
    /// <summary>
    /// Interaction logic for ServerLog.xaml
    /// </summary>
    public partial class ServerLog : UserControl
    {
        protected IServerLogViewModel ViewModel => DataContext as IServerLogViewModel;
        public ServerLog()
        {
            InitializeComponent();
            LogList.SizeChanged += ScrollDown;
        }

        private void ScrollDown(object sender, System.Windows.SizeChangedEventArgs e)
        {
            LogScroller.ScrollToBottom();
        }
    }
}
