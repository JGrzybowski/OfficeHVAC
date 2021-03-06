﻿using Akka.Actor;
using NodaTime;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using Prism.Mvvm;
using System.Timers;

namespace OfficeHVAC.Modules.TimeSimulation.ViewModels
{
    public class TimeControlViewModel : BindableBase, ITimeControlViewModel
    {
        public const string TimeSimulatorActorName = "Clock";

        private readonly Timer timer;
        public IActorRef Bridge { get; }

        private Instant time;
        public Instant Time
        {
            get => time;
            set
            {
                SetProperty(ref time, value);
                RaisePropertyChanged(nameof(TimeText));
            }
        }
        public string TimeText => Time.ToString("hh:mm:ss", null);

        public bool IsRunning => timer.Enabled;
        
        private double speed = 10;
        public double Speed
        {
            get => speed;
            set => Bridge.Tell(new SetSpeedMessage(value));
        }
        public void SetupSpeed(double newValue) => SetProperty(ref speed, newValue, nameof(Speed));

        public TimeControlViewModel(IControlledTimeSource controlledTimeSource, ActorSystem actorSystem, long timerRefreshRate = 1000)
        {
            var timeSimulatorActorRef = actorSystem.ActorOf(TimeSimulatorActor.Props(controlledTimeSource), TimeSimulatorActorName);

            var bridgeProps = TimeSimulatorBridgeActor.Props(this, timeSimulatorActorRef);
            Bridge = actorSystem.ActorOf(bridgeProps, "timeBridge");

            timer = new Timer(timerRefreshRate) { AutoReset = true };
            timer.Elapsed += TimerTick;
        }

        public void TimerTick(object sender, ElapsedEventArgs elapsedEventArgs) => TickManually();

        public void ToggleTimer()
        {
            if (timer.Enabled)
                timer.Stop();
            else
                timer.Start();

            RaisePropertyChanged(nameof(IsRunning));
        }

        public void AddMinutes(long minutes) => Bridge.Tell(new AddMinutesMessage(minutes));

        public void TickManually() => Bridge.Tell(new TickClockMessage());
    }
}
