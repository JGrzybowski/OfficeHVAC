using Akka.Actor;
using OfficeHVAC.Applications.BuildingSimulator.ViewModels;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.RoomSimulator.Actors;

namespace OfficeHVAC.Applications.BuildingSimulator.Actors {
    public class TemperatureActuatorBridgeActor : BridgeActor<SensorViewModel<TemperatureActuatorActor.Status,double>>
    {
        public TemperatureActuatorBridgeActor(SensorViewModel<TemperatureActuatorActor.Status, double> viewModel, IActorRef actorRef)
            : base(viewModel, actorRef)
        {
            
        }

        public static Props Props(TemperatureActuatorViewModel viewModel, IActorRef temperatureActuatorActor) => 
            Akka.Actor.Props.Create(() => new TemperatureActuatorBridgeActor(viewModel, temperatureActuatorActor));
    }
}