using OfficeHVAC.Models;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class TemperatureSensorViewModel : SensorViewModel<double>
    {
        public TemperatureSensorViewModel()
        {
            SensorType = SensorType.Temperature;
        }
    }
}