using NodaTime;
using OfficeHVAC.Models;

namespace OfficeHVAC.Modules.TemperatureSimulation
{
    public interface ITemperatureSimulator
    {
        double ChangeTemperature(IRoomStatusMessage status, Duration timeDelta);

        double Temperature { get; set; }
    }
}