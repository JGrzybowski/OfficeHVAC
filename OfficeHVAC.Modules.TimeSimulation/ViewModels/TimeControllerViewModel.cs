using NodaTime;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using Prism.Mvvm;
using System.Timers;

namespace OfficeHVAC.Modules.TimeSimulation.ViewModels
{
    public class TimeControllerViewModel : BindableBase, ITimeControllerViewModel
    {
        private IControlledTimeSource controlledTimeSource;

        public TimeControllerViewModel(IControlledTimeSource controlledTimeSource)
        {
            this.controlledTimeSource = controlledTimeSource;
        }

        public double Speed { get; set; }

        public Instant ResetTime { get; set; }

        public void AddMinutes(long minutes)
        {
        }

        public void Reset()
        {
        }

        public void TickManually()
        {
        }

        public void ToggleTimer()
        {
        }

        public void TimerTick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            TickManually();
        }
    }
}
