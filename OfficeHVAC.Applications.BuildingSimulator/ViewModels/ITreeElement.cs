using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public interface ITreeElement : INotifyPropertyChanged
    {
        string Id { get; }

        string Name { get; set; }

        ObservableCollection<ITreeElement> SubItems { get; }
    }
}
