using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class TemperatureControllerViewModel : SensorViewModel<TemperatureControllerStatus, double>
    {
        private Duration buffer;
        public Duration Buffer
        {
            get => buffer;
            protected set => SetProperty(ref buffer, value);
        }

        public IEnumerable<ITemperatureDeviceStatus> Devices
        {
            get => SubItems.Cast<ITemperatureDeviceStatus>();
            protected set
            {
                var vms = value.Select(d => new DeviceViewModel(d));
                SubItems= new ObservableCollection<ITreeElement>(vms);
                RaisePropertyChanged(nameof(Devices));
                RaisePropertyChanged(nameof(SubItems));
            }
        }

        public TemperatureControllerViewModel()
        {
            SensorType = SensorType.Temperature;
        }
      
        public override void PushStatus(TemperatureControllerStatus status)
        {
            SetProperty(ref timestamp, status.Timestamp, nameof(Timestamp));
            SetProperty(ref paramValue, status.ParameterValue, nameof(ParamValue));
            SetProperty(ref buffer, status.TheresholdBuffer, nameof(Buffer));
            Devices = status.Devices;
        }
    }
}