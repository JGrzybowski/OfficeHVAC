using NodaTime;

namespace OfficeHVAC.Modules.TimeSimulation.ViewModels
{
    public interface ITimeControlViewModel
    {
        double Speed { get; set; }
        void SetupSpeed(double newValue);

        bool IsRunning { get; }

        Instant Time { get; set; }
        string TimeText { get; }

        void AddMinutes(long minutes);
        void TickManually();
        void ToggleTimer();
    }
}