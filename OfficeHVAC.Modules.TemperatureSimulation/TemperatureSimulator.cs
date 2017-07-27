using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System.Linq;

namespace OfficeHVAC.Modules.TemperatureSimulation
{
    public class TemperatureSimulator : ITemperatureSimulator
    {
        public ITemperatureModel Model { get; }

        protected double LastTemperature;
        public double Temperature
        {
            get { return LastTemperature; }
            set { LastTemperature = value; }
        }

        public TemperatureSimulator(double initialTemperature, ITemperatureModel model)
        {
            this.Model = model;
            this.LastTemperature = initialTemperature;
        }

        public double ChangeTemperature(IRoomStatusMessage status, Duration timeDelta)
        {
            var temperatureDevices =
                status.Devices
                        .Where(dev => dev is ITemperatureDevice)
                        .Cast<ITemperatureDevice>()
                        .ToList();

            LastTemperature += Model.CalculateChange(LastTemperature, temperatureDevices, timeDelta, status.Volume);
            return LastTemperature;
        }
    }
}
