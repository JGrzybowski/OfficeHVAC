using Akka.Actor;
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
        private string _timeText;

        public Instant ResetTime { get; set; }

        public bool IsRunning => _timer.Enabled;
        
        //TODO Store TimeController State
        public double Speed
        {
            get =>_controlledTimeSource.Speed;
            set
            {
                _controlledTimeSource.Speed = value;
                this.RaisePropertyChanged();
            }
        }

        public string TimeText
        {
            get => _timeText;
            set => SetProperty(ref _timeText, value);
        }

        public TimeControlViewModel(IControlledTimeSource controlledTimeSource, ActorSystem actorSystem, long timerRefreshRate = 1000)
        {
            var bridgeProps = Props.Create(() => new TimeSimulatorBridgeActor(this, controlledTimeSource));
            this.Bridge = actorSystem.ActorOf(bridgeProps, "timeBridge");

            _timer = new Timer(timerRefreshRate) { AutoReset = true };
            _timer.Elapsed += TimerTick;
        }

        public IActorRef Bridge { get; }

        public void TimerTick(object sender, ElapsedEventArgs elapsedEventArgs) => TickManually();

        public void ToggleTimer()
        {
            if (_timer.Enabled)
                _timer.Stop();
            else
                _timer.Start();

            RaisePropertyChanged(nameof(IsRunning));
        }

        public void AddMinutes(long minutes)
        {
            _controlledTimeSource.AddMinutes(minutes);
            RaisePropertyChanged(nameof(TimeText));
        }

        public void TickManually()
        {
            _controlledTimeSource.UpdateClock();
            RaisePropertyChanged(nameof(TimeText));
        }

        public void Reset()
        {
            _controlledTimeSource.Reset(ResetTime);
            RaisePropertyChanged(nameof(TimeText));
        }
    }
}
