using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class DeviceViewModel : BindableBase, ITreeElement
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public int MaxPower { get; }
        public IEnumerable<ITemperatureMode> Modes { get; }
        public double DesiredTemperature { get; }
        public TemperatureModeType ActiveModeType { get; }

        public ObservableCollection<ITreeElement> SubItems { get; }

        public DeviceViewModel() { }

        public DeviceViewModel(ITemperatureDeviceStatus status)
        {
            Id = status.Id;
            MaxPower = status.MaxPower;
            Modes = status.Modes;
            ActiveModeType = status.ActiveModeType;
            DesiredTemperature = status.DesiredTemperature;
        }
    }
}
