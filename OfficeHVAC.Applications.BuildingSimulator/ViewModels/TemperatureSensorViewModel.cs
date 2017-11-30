using Akka.Util;
using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation.Actors;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class TemperatureSensorViewModel : SensorViewModel<TemperatureSimulatorActorStatus,double>
    {
        private Duration buffer;
        public Duration Buffer
        {
                      get => buffer;
            protected set => SetProperty(ref buffer, value);
        }

        public TemperatureSensorViewModel()
        {
            SensorType = SensorType.Temperature;
        }

        public override void PushStatus(TemperatureSimulatorActorStatus status)
        {
            SetProperty(ref timestamp, status.Timestamp, nameof(Timestamp));
            SetProperty(ref paramValue, status.ParameterValue, nameof(ParamValue));
            SetProperty(ref buffer, status.TheresholdBuffer, nameof(Buffer));
        }
    }
}