using Akka.Actor;
using OfficeHVAC.Applications.BuildingSimulator.ViewModels;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;

namespace OfficeHVAC.Applications.BuildingSimulator.Actors
{
    public class TemperatureSimulatorBridgeActor : BridgeActor<SensorViewModel<TemperatureSimulatorActorStatus,double>>
    {
        public TemperatureSimulatorBridgeActor(SensorViewModel<TemperatureSimulatorActorStatus,double> viewModel, IActorRef actorRef) 
            : base(viewModel, actorRef)
        {
            Receive<TemperatureSimulatorActorStatus>(msg => UpdateViewModel(msg));
            actorRef.Tell(new DebugSubscribeMessage(Self));
        }

        private void UpdateViewModel(TemperatureSimulatorActorStatus status)
        {
            ViewModel.PushStatus(status);
        }

        public static Props Props(TemperatureSensorViewModel sensor, IActorRef tempSensorActor) => 
            Akka.Actor.Props.Create(() => new TemperatureSimulatorBridgeActor(sensor, tempSensorActor));
    }
}