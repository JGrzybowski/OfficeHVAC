using NodaTime;

namespace OfficeHVAC.Modules.TimeSimulation.ViewModels
{
    public interface ITimeControllerViewModel
    {
        Instant ResetTime { get; set; }
        double Speed { get; set; }

        void AddMinutes(long minutes);
        void Reset();
        void TickManually();
        void ToggleTimer();
    }
}