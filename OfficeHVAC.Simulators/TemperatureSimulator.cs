using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeHVAC.Models;

namespace OfficeHVAC.Simulators
{
    public class TemperatureSimulator : ITemperatureSimulator
    {
        
        private readonly Func<float, IEnumerable<IDevice>, TimeSpan, float> temperatureCalculationModel;
        private float lastTemperature;
        private readonly DateTime lastTime;

        public TemperatureSimulator(
            Func<float, IEnumerable<IDevice>, TimeSpan, float> temperatureCalculationModel, 
            ITimeSource timeSource,
            float initialTemperature = 25
            )
        {
            throw new NotImplementedException();
            this.TimeSource = timeSource;
            this.lastTime = timeSource.Time;
            this.temperatureCalculationModel = temperatureCalculationModel;
            this.lastTemperature = initialTemperature;
        }

        public IEnumerable<IDevice> Devices { get; set; } = new List<IDevice>();
        public ITimeSource TimeSource { get; set; }
        public float Temperature 
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

    }
}
