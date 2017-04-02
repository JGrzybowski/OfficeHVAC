using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeHVAC.Models
{
    public class SimulatorModels : ISimulatorModels
    {
        public ITimeSource TimeSource => timeSource;
        private ITimeSource timeSource;

        public ITemperatureModel TemperatureModel => temperatureModel;
        private ITemperatureModel temperatureModel;

        public SimulatorModels(ITimeSource timeSource, ITemperatureModel temperatureModel)
        {
            this.timeSource = timeSource;
            this.temperatureModel = temperatureModel;
        }
    }
}
