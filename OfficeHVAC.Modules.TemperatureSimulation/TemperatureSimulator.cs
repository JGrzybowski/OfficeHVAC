using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Modules.TemperatureSimulation
{
    public class TemperatureSimulator : ITemperatureSimulator
    {
        public ITemperatureModel Model { get; }
        protected double LastTemperature { get; set; }
        protected Instant LastTime { get; set; }

        public TemperatureSimulator(ISimulatorModels models, double initialTemperature)
            : this(models.TimeSource, initialTemperature, models.TemperatureModel) { }

        public TemperatureSimulator(ITimeSource timeSource, double initialTemperature, ITemperatureModel model)
        {
            this.Model = model;
            this.TimeSource = timeSource;
            this.LastTime = timeSource.Now;
            this.LastTemperature = initialTemperature;
        }

        public ITimeSource TimeSource { get; }
        public double RoomVolume { get; set; }

        public double GetTemperature(IRoomStatusMessage status)
        {
            var now = this.TimeSource.Now;
            var timeDelta = (now - LastTime);
            LastTime = now;

            var temperatureDevices =
                status.Devices
                        .Where(dev => dev is ITemperatureDevice)
                        .Cast<ITemperatureDevice>()
                        .ToList();

            LastTemperature += Model.CalculateChange(LastTemperature, temperatureDevices, timeDelta, status.Volume);
            return LastTemperature;
        }

        public void SetTemperature(double value)
        {
            this.LastTemperature = value;
        }
    }
}
