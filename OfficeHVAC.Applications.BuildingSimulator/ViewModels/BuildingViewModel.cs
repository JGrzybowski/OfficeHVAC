using OfficeHVAC.Modules.TimeSimulation.TimeSources;
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

        private IControlledTimeSource timeSource;
        public IControlledTimeSource TimeSource
        {
            get => timeSource;
            set => SetProperty(ref timeSource, value);
        }

        public string NewCompanyName { get; set; }

        public CompanyViewModel SelectedCompany { get; set; }

        public ICommand AddCompanyCommand { get; set; }
        public ICommand RemoveCompanyCommand { get; set; }


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
