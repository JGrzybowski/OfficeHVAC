using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation;

namespace OfficeHVAC.Applications.RoomSimulator
{
    public class SimulatorModels : ISimulatorModels
    {
        public ITemperatureModel TemperatureModel => new SimpleTemperatureModel();

        public SimulatorModels() {        }
    }
}
