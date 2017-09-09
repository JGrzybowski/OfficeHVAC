using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    class BuildingViewModel : BindableBase
    {
        public ObservableCollection<CompanyViewModel> Companies { get; set; } =
            new ObservableCollection<CompanyViewModel>();
        
        public string NewCompanyName { get; set; }

        public CompanyViewModel SelectedCompany { get; set; }

        public ICommand AddCompanyCommand { get; set; }
        public ICommand RemoveCompanyCommand { get; set; }

        private TimeControlViewModel timeControlViewModel;
        public TimeControlViewModel TimeControlViewModel
        {
            get => timeControlViewModel;
            set => SetProperty(ref timeControlViewModel, value);
        }


        public BuildingViewModel()
        {
            AddCompanyCommand = new DelegateCommand(
                () => Companies.Add(new CompanyViewModel(NewCompanyName)),
                () => !String.IsNullOrWhiteSpace(NewCompanyName)
            );

            RemoveCompanyCommand = new DelegateCommand(
                () => Companies.Remove(SelectedCompany),
                () => SelectedCompany != null
            );

            NewCompanyName = "Cmp";
        }


    }
}
