using System.CodeDom.Compiler;
using System.ComponentModel;
using Akka.Actor;
using OfficeHVAC.Applications.BuildingSimulator.Actors;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation;
using Prism.Mvvm;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels 
{
    public class TemperatureSimulatorParamsViewModel : BindableBase
    {
        public const string TempSimulatorModelActorName = "TempParams";
        
        private double airsSpecificHeat = 1005;   //  W / kg*C*s
        private double airsDensity = 1.2;         // kg / m^3 
        private IActorRef actor;

        public TemperatureSimulatorParamsViewModel(ActorSystem actorSystem)
        {
            actor = actorSystem.ActorOf<TemperatureModelActor>(SystemInfo.TempSimulatorModelActorName);
            actor.Tell(Model);
            
            PropertyChanged += OnParamsChange; 
        }

        private TemperatureModelParams ModelParams => new TemperatureModelParams(AirsSpecificHeat, AirsDensity);
        private ITemperatureModel Model = new SimpleTemperatureModel();

        private void OnParamsChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(AirsDensity) && e.PropertyName != nameof(AirsSpecificHeat)) 
                return;
            
            Model = new SimpleTemperatureModel();
            actor.Tell(Model);
        }
        
        public double AirsSpecificHeat
        {
            get => airsSpecificHeat;
            set => SetProperty(ref airsSpecificHeat, value);
        }

        public double AirsDensity
        {
            get => airsDensity;
            set => SetProperty(ref airsDensity, value);
        } 
    }
}