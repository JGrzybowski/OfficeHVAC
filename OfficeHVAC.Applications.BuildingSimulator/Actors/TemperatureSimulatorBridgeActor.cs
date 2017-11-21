using Akka.Actor;
using OfficeHVAC.Applications.BuildingSimulator.ViewModels;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;

namespace OfficeHVAC.Applications.BuildingSimulator.Actors
{
    public class TemperatureSimulatorBridgeActor : BridgeActor<SensorViewModel<double>>
    {
        public TemperatureSimulatorBridgeActor(SensorViewModel<double> viewModel, IActorRef actorRef) 
            : base(viewModel, actorRef)
        {
            Receive<TemperatureSimulatorActor.Status>(msg => UpdateViewModel(msg));
            actorRef.Tell(new DebugSubscribeMessage(Self));
        }

        private void UpdateViewModel(TemperatureSimulatorActor.Status status)
        {
            ViewModel.PushParam(status.Temperature);
            ViewModel.Timestamp = status.LastTimestamp;
        }

        public static Props Props(TemperatureSensorViewModel sensor, IActorRef tempSensorActor) => 
            Akka.Actor.Props.Create(() => new TemperatureSimulatorBridgeActor(sensor, tempSensorActor));
    }
}