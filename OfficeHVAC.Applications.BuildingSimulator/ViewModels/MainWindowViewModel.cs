using Akka.Actor;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using Prism.Mvvm;
using System.Threading.Tasks;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private bool isSimulatorRunning;
        public bool IsSimulatorRunning
        {
            get => isSimulatorRunning;
            set
            {
                isSimulatorRunning = value;
                RaisePropertyChanged(nameof(IsSimulatorRunning));
                RaisePropertyChanged(nameof(IsSimulatorStopped));
            }
        }
        public bool IsSimulatorStopped => !isSimulatorRunning;  

        public ActorSystem ActorSystem { get; set; }

        public BuildingViewModel Building { get; set; }
        public ITimeControlViewModel Time { get; set; }
        
        public MainWindowViewModel(ActorSystem actorSystem, BuildingViewModel building, ITimeControlViewModel time)
        {
            ActorSystem = actorSystem;
            Building = building;
            Time = time;


            //SeedData().Wait();
        }

        private async Task SeedData()
        {
            var company = Building.AddCompany();
            company.Name = "CompanyA";
            
            var room = await company.AddRoom();
            room.Name = "Room 101";
            room = await company.AddRoom();
            room.Name = "Room 102";
                
            company = Building.AddCompany();
            company.Name = "CompanyB";
            
            room = await company.AddRoom();
            room.Name = "Room 1053";
            room = await company.AddRoom();
            room.Name = "Room 1026";
            
            company = Building.AddCompany();
            company.Name = "CompanyC";
            
            room = await company.AddRoom();
            room.Name = "Room 57";
            room = await company.AddRoom();
            room.Name = "Room 1021";
        }
    }
}
