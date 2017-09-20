using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class DeviceViewModel : BindableBase, ITreeElement
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public ObservableCollection<ITreeElement> SubItems { get; }
    }
}
