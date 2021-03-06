﻿using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace OfficeHVAC.Modules.TimeSimulation.Views
{
    /// <summary>
    /// Interaction logic for TimeControl.xaml
    /// </summary>
    public partial class TimeControl : UserControl
    {
        private ITimeControlViewModel ViewModel => DataContext as ITimeControlViewModel;

        public TimeControl()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) => ViewModel.ToggleTimer();

        private void Plus5minButton_Click(object sender, RoutedEventArgs e) => ViewModel.AddMinutes(5);

        private void Plus15minButton_Click(object sender, RoutedEventArgs e) => ViewModel.AddMinutes(15);

        private void Plus1hrButton_Click(object sender, RoutedEventArgs e) => ViewModel.AddMinutes(60);

        private void TickButton_Click(object sender, RoutedEventArgs e) => ViewModel.TickManually();
    }
}
