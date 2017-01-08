using NodaTime;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using Prism.Mvvm;
using System.Timers;

namespace OfficeHVAC.Modules.TimeSimulation.ViewModels
{
    public class TimeControlViewModel : BindableBase, ITimeControlViewModel
    {
        private readonly IControlledTimeSource _controlledTimeSource;
        private readonly Timer _timer;
        
        public Instant ResetTime { get; set; }

        public bool IsRunning => _timer.Enabled;

        public double Speed
        {
            get { return _controlledTimeSource.Speed; }
            set
            {
                _controlledTimeSource.Speed = value;
                this.OnPropertyChanged();
            }
        }

        public string TimeText => _controlledTimeSource.Now.ToString();

        public TimeControlViewModel(IControlledTimeSource controlledTimeSource, long timerRefreshRate = 1000)
        {
            _controlledTimeSource = controlledTimeSource;
            _timer = new Timer(timerRefreshRate) { AutoReset = true };
            _timer.Elapsed += TimerTick;
        }
        
        public void TimerTick(object sender, ElapsedEventArgs elapsedEventArgs) => TickManually();

        public void ToggleTimer()
        {
            if (_timer.Enabled)
                _timer.Stop();
            else
                _timer.Start();

            OnPropertyChanged(nameof(IsRunning));
        }

        public void AddMinutes(long minutes)
        {
            _controlledTimeSource.AddMinutes(minutes);
            OnPropertyChanged(nameof(TimeText));
        }

        public void TickManually()
        {
            _controlledTimeSource.UpdateClock();
            OnPropertyChanged(nameof(TimeText));
        }

        public void Reset()
        {
            _controlledTimeSource.Reset(ResetTime);
            OnPropertyChanged(nameof(TimeText));
        }
    }
}
