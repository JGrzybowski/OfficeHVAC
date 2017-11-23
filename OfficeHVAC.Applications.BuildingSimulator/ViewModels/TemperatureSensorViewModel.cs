using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class TemperatureSensorViewModel : SensorViewModel<TemperatureSimulatorActor.Status,double>
    {
        public TemperatureSensorViewModel()
        {
            SensorType = SensorType.Temperature;
        }

        public override void PushStatus(TemperatureSimulatorActor.Status status)
        {
            SetProperty(ref timestamp, status.LastTimestamp, nameof(Timestamp));
            SetProperty(ref paramValue, status.Temperature, nameof(ParamValue));
        }
    }
}