using OfficeHVAC.Modules.RoomSimulator.Actors;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels {
    public class TemperatureActuatorViewModel : SensorViewModel<TemperatureActuatorActor.Status, double>
    {
        public override void PushStatus(TemperatureActuatorActor.Status status)
        {
            throw new System.NotImplementedException();
        }
    }
}