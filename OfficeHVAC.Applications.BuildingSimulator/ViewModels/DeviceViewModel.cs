using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
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
        public ITemperatureMode ActiveMode => Modes.SingleOrDefault(m => m.Type == ActiveModeType);
        public double CurrentPowerUsage => ActiveMode?.CalculateEffectivePower(MaxPower) ?? 0;

        public ObservableCollection<ITreeElement> SubItems { get; }

        public DeviceViewModel() { }

        public DeviceViewModel(ITemperatureDeviceStatus status)
        {
            Name = "HVAC Device";
            Id = status.Id;
            MaxPower = status.MaxPower;
            Modes = status.Modes;
            ActiveModeType = status.ActiveModeType;
            DesiredTemperature = status.GetDesiredTemperature();
            SubItems = new ObservableCollection<ITreeElement>(status.Modes.Select(m => new TemperatureModeViewModel(m)));
        }
    }
}
