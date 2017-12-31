using System.Collections.Generic;
using Akka.Actor;
using OfficeHVAC.Applications.BuildingSimulator.ViewModels;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;

namespace OfficeHVAC.Applications.BuildingSimulator.Actors {
    public class TemperatureControllerBridgeActor : BridgeActor<SensorViewModel<TemperatureControllerStatus,double>>
    {
        public TemperatureControllerBridgeActor(SensorViewModel<TemperatureControllerStatus, double> viewModel, IActorRef actorRef)
            : base(viewModel, actorRef)
        {
            Receive<TemperatureControllerStatus>(msg => UpdateViewModel(msg));
            actorRef.Tell(new DebugSubscribeMessage(Self));
        }

        private void UpdateViewModel(TemperatureControllerStatus status)
        {
            ViewModel.PushStatus(status);
        }

        public static Props Props(TemperatureControllerViewModel viewModel, IActorRef temperatureActuatorActor) => 
            Akka.Actor.Props.Create(() => new TemperatureControllerBridgeActor(viewModel, temperatureActuatorActor));
    }
}