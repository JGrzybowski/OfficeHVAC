using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class TemperatureSensorViewModel : SensorViewModel<TemperatureSimulatorActorStatus,double>
    {
        public TemperatureSensorViewModel()
        {
            SensorType = SensorType.Temperature;
        }

        public override void PushStatus(TemperatureSimulatorActorStatus status)
        {
            SetProperty(ref timestamp, status.Timestamp, nameof(Timestamp));
            SetProperty(ref paramValue, status.Temperature, nameof(ParamValue));
        }
    }
}