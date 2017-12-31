using System.Collections.ObjectModel;
using System.ComponentModel;
using OfficeHVAC.Models.Devices;
using Prism.Mvvm;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    //TODO SetProperty once Allowed Editing Modes
    public class TemperatureModeViewModel : BindableBase, ITreeElement
    {
        public string Id { get; }
        
        public string Name { get; set; }
        public ObservableCollection<ITreeElement> SubItems { get; }
        public TemperatureModeType Type { get; set; }
        public double PowerConsumption { get; set; }
        public double PowerEfficiency { get; set; }
        public double EffectivePower => PowerConsumption* PowerEfficiency;
        
        public TemperatureModeViewModel(ITemperatureMode mode)
        {
            Type = mode.Type;
            Name = mode.Name;
            PowerConsumption = mode.PowerConsumption;
            PowerEfficiency = mode.PowerEfficiency;
        }
    }
}