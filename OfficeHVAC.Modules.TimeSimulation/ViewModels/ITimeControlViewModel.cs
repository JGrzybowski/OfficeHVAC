using NodaTime;

namespace OfficeHVAC.Modules.TimeSimulation.ViewModels
{
    public interface ITimeControlViewModel
    {
        Instant ResetTime { get; set; }

        double Speed { get; set; }

        bool IsRunning { get; }

        string TimeText { get; set; }

        void AddMinutes(long minutes);
        void Reset();
        void TickManually();
        void ToggleTimer();
    }
}