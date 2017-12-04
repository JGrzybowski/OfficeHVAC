using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace OfficeHVAC.Modules.TemperatureSimulation.Messages {
    public class AddTemperatureActuatorMessage
    {
        public string Id { get; }
        public IEnumerable<string> SubsriptionSources { get; }

        public AddTemperatureActuatorMessage(string id, IEnumerable<string> subsriptionSources)
        {
            Id = id;
            SubsriptionSources = subsriptionSources;
        }
    }
}