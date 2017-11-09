using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels {
    class TreeElement : BindableBase, ITreeElement {
        public string Id { get; }
        public string Name { get; set; }
        public ObservableCollection<ITreeElement> SubItems { get; }

        public TreeElement(ObservableCollection<ITreeElement> subItems, string Id = null)
        {
            SubItems = subItems;
        }
    }
}