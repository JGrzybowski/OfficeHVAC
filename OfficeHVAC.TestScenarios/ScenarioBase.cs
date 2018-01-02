using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NodaTime;
using OfficeHVAC.Applications.BuildingSimulator.Actors;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.RoomSimulator.Actors;
using OfficeHVAC.Modules.TemperatureSimulation;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace OfficeHVAC.TestScenarios
{
    public class ScenarioBase : TestKit
    {
        protected IActorRef Hole;

        protected Instant Date;

        protected IControlledTimeSource TimeSource;
        protected IActorRef TimeSourceActor;
        protected string TimeSourceActorPath;

        protected ITemperatureModel TemperatureModel;
        protected IActorRef ModelsActor;
        protected string ModelsActorPath;

        protected TestActorRef<RoomSimulatorActor> RoomActorRef;

        protected List<Expectation> Meetings = new List<Expectation>();

        protected double TimeScaleInSeconds = 15;

        protected TemperatureDeviceDefinition deviceDefinition = new TemperatureDeviceDefinition
        (
            id: "Test dev",
            maxPower: 1000,
            modes: new ModesCollection
            {
                new TemperatureMode()
                {
                    Name = "Off",
                    Type = TemperatureModeType.Off,
                    TemperatureRange = new Range<double>(-100, 100)
                },
                new TemperatureMode()
                {
                    Name = "Stabilization",
                    Type = TemperatureModeType.Stabilization,
                    PowerEfficiency = 0.99,
                    PowerConsumption = 0.1,
                    TemperatureRange = new Range<double>(-3, 3)
                },
                new TemperatureMode()
                {
                    Name = "Eco",
                    Type = TemperatureModeType.Eco,
                    PowerEfficiency = 0.93,
                    PowerConsumption = 0.3,
                    TemperatureRange = new Range<double>(-100, 100)
                },
                new TemperatureMode()
                {
                    Name = "Turbo",
                    Type = TemperatureModeType.Turbo,
                    PowerEfficiency = 0.40,
                    PowerConsumption = 1.0,
                    TemperatureRange = new Range<double>(-100, 100)
                }
            }
        );

        protected RoomStatus initialStatus;

        public ScenarioBase(ITestOutputHelper output) : base(@"akka.scheduler.implementation = ""Akka.TestKit.TestScheduler, Akka.TestKit""", output: output)
        {
            Hole = ActorOf<BlackHoleActor>();

            Date = Instant.FromDateTimeUtc(DateTime.UtcNow.AddDays(2).Date);
            
            var testScheduler = Sys.Scheduler as TestScheduler;
            TimeSource = new ControlledTimeSource(Date, testScheduler);
            TimeSourceActor = Sys.ActorOf(TimeSimulatorActor.Props(TimeSource), "time");
            TimeSourceActorPath = TimeSourceActor.Path.ToStringWithoutAddress();
            TimeSourceActor.Tell(new SetSpeedMessage(TimeScaleInSeconds));

            TemperatureModel = new SimpleTemperatureModel();
            ModelsActor = Sys.ActorOf<TemperatureModelActor>("models");
            ModelsActor.Tell(TemperatureModel);
            ModelsActorPath = ModelsActor.Path.ToStringWithoutAddress();

            initialStatus = new RoomStatus()
            {
                Name = "Test Room",
                Volume = 72
            };
            initialStatus.Parameters.Add(new ParameterValue(SensorType.Temperature, 25));
        }

        protected void InitializeRoom()
        {
            RoomActorRef = ActorOfAsTestActorRef(() => new RoomSimulatorActor(initialStatus, Hole.Path), "room");
            TimeSourceActor.Tell(new SubscribeMessage(RoomActorRef));

            RoomActorRef.Tell(new AddTemperatureSensorMessage(
                TimeSourceActor.Path.ToStringWithoutAddress(),
                ModelsActor.Path.ToStringWithoutAddress(),
                "Temp-sensor"));

            RoomActorRef.Tell(new AddTemperatureActuatorMessage("Temp-controller", new[] { TimeSourceActorPath, ModelsActorPath }));
            RoomActorRef.Tell(new AddDevice<ITemperatureDeviceDefinition>(deviceDefinition));
        }
    
        protected void InitializeTimerTo(int hours, int minutes)
        {
            TimeSource.Reset(At(hours, minutes));
        }

        protected void DevicesShouldBeTurnedOff()
        {
            var status = GetStatus();
            var all = status.TemperatureDevices.All(d => d.ActiveModeType == TemperatureModeType.Off)
                    ? "wszystkie" 
                    : "nie wszystkie";
            Output?.WriteLine($"{TimeSource.Now.ToDateTimeUtc().Hour:00}:{TimeSource.Now.ToDateTimeUtc().Minute:00} - {all} urządzenia są wyłączone.");
            status.TemperatureDevices.ShouldAllBe(dev => dev.ActiveModeType == TemperatureModeType.Off || dev.ActiveModeType == TemperatureModeType.StandBy);
        }

        protected void TemperatureShouldBe(double expectedTemperature, double tolerance = 0.5)
        {
            Thread.Sleep(500);
            var status = GetStatus();
            var temperature = Convert.ToDouble(status.Parameters[SensorType.Temperature].Value);
            Output?.WriteLine($"{TimeSource.Now.ToDateTimeUtc().Hour:00}:{TimeSource.Now.ToDateTimeUtc().Minute:00} - w sali panowała temperatura {temperature:F}st.C (oczekiwana {expectedTemperature}st.C)");
            temperature.ShouldBe(expectedTemperature, tolerance);
        }

        protected IRoomStatusMessage GetStatus()
        {
            return RoomActorRef.UnderlyingActor.Status;
            //RoomActorRef.Tell(new RoomStatus.Request());
            //var status = ExpectMsg<RoomStatus>();
            //return status;
        }

        protected Expectation SetMeeting(Instant time, Instant endTime, params ParameterValue[] parameters)
            => AddMeeting(time, endTime, parameters);

        protected Expectation AddMeeting(Instant time, Instant endTime, IEnumerable<ParameterValue> parameters)
        {
            var expectation = new Expectation(time, endTime, parameters);
            Meetings.Add(expectation);
            UpdateMeetings();
            return expectation;
        }

        protected void CancelMeeting(Instant time)
        {
            Meetings.RemoveAll(m => m.From == time);
            UpdateMeetings();
        }

        protected Expectation PostponeMeeting(Instant time, Instant newStart, Instant newEnd)
        {
            var oldMeeting = Meetings.First(m => m.From == time);
            Meetings.RemoveAll(m => m.From == time);
            return AddMeeting(newStart, newEnd, oldMeeting.ExpectedParametersValues);
        }

        protected Expectation ExtendMeeting(Instant time, Instant newEnd)
        {
            var oldMeeting = Meetings.First(m => m.From == time);
            Meetings.RemoveAll(m => m.From == time);
            return AddMeeting(oldMeeting.From, newEnd, oldMeeting.ExpectedParametersValues);
        }

        protected void UpdateMeetings() => RoomActorRef.Tell(Meetings.ToArray());

        protected void MoveTimeTo(int hours, int minutes)
        {
            double repetitions = (At(hours, minutes) - TimeSource.Now).TotalSeconds;
            repetitions /= TimeScaleInSeconds;

            for (int i = 0; i < repetitions; i++)
            {
                TimeSourceActor.Tell(new TickClockMessage());
                Thread.Sleep(5);

                var T = Convert.ToDouble(RoomActorRef.UnderlyingActor.Status.Parameters[SensorType.Temperature].Value);
                Debug.WriteLine($"at {TimeSource.Now} T is {T}");
            }

            TimeSource.Reset(At(hours, minutes));
        }

        protected Instant At(int hours, int minutes = 0, int seconds = 0) => Date
                                                                           + Duration.FromHours(hours)
                                                                           + Duration.FromMinutes(minutes)
                                                                           + Duration.FromSeconds(seconds);

        protected static ParameterValue Temperature(double t) =>
            new ParameterValue(SensorType.Temperature, t);

        protected static ParameterValuesCollection TemperatureOf(double t) =>
            new ParameterValuesCollection() { Temperature(t) };
    }
}
