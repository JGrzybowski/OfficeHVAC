using NodaTime;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using Prism.Mvvm;
using System.Timers;

namespace OfficeHVAC.Modules.TimeSimulation.ViewModels
{
    public class TimeControllerViewModel : BindableBase, ITimeControllerViewModel
    {
        private readonly IControlledTimeSource _controlledTimeSource;
        private readonly Timer _timer;
        
        public TimeControllerViewModel(IControlledTimeSource controlledTimeSource, long timerRefreshRate = 1000)
        {
            _controlledTimeSource = controlledTimeSource;
            _timer = new Timer(timerRefreshRate) { AutoReset = true };
            _timer.Elapsed += TimerTick;
        }

        public Instant ResetTime { get; set; }

        public double Speed
        {
            get { return _controlledTimeSource.Speed; }
            set
            {
                _controlledTimeSource.Speed = value;
                this.OnPropertyChanged();
            }
        }

        public void TimerTick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            TickManually();
        }

        public void ToggleTimer()
        {
            if (_timer.Enabled)
                _timer.Stop();
            else
                _timer.Start();
        }

        public void AddMinutes(long minutes)
        {
            _controlledTimeSource.AddMinutes(minutes);
        }

        public void TickManually()
        {
            _controlledTimeSource.UpdateClock();
        }

        public void Reset()
        {
            _controlledTimeSource.Reset(ResetTime);
        }

    }
}
