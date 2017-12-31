using NodaTime;
using OfficeHVAC.Models;
using System.Linq;

namespace OfficeHVAC.Modules.TemperatureSimulation
{
    public class TemperatureSimulator : ITemperatureSimulator
    {
        public ITemperatureModel Model { get; protected set; }

        protected double LastTemperature;
        public double Temperature
        {
            get => LastTemperature;
            set => LastTemperature = value;
        }

        public void ReplaceTemperatureModel(ITemperatureModel model) => Model = model;

        public TemperatureSimulator(double initialTemperature, ITemperatureModel model = null)
        {
            Model = model;
            LastTemperature = initialTemperature;
        }

        public double ChangeTemperature(IRoomStatusMessage status, Duration timeDelta)
        {
            var temperatureDevices =
                status.TemperatureDevices
                        .ToList();

            LastTemperature += Model?.CalculateChange(LastTemperature, temperatureDevices, timeDelta, status.Volume) ?? 0; 
            return LastTemperature;
        }
    }
}
