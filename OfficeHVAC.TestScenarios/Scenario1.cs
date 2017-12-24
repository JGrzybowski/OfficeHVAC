using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NodaTime;
using OfficeHVAC.Applications.BuildingSimulator.Actors;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Modules.RoomSimulator.Actors;
using OfficeHVAC.Modules.TemperatureSimulation;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using Shouldly;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace OfficeHVAC.TestScenarios
{
    public class Scenario1 : TestKit
    {
        private IActorRef hole;

        private Instant date;

        private IControlledTimeSource timeSource;
        private IActorRef timeSourceActor;
        private string timeSourceActorPath;

        private ITemperatureModel temperatureModel = new SimpleTemperatureModel();
        private IActorRef modelsActor;
        private string modelsActorPath;

        private TestActorRef<RoomSimulatorActor> roomActorRef;

        public Scenario1() : base(@"akka.scheduler.implementation = ""Akka.TestKit.TestScheduler, Akka.TestKit""")
        {
            hole = ActorOf<BlackHoleActor>();

            date = Instant.FromDateTimeUtc(DateTime.UtcNow.AddDays(2).Date);
            
            var testScheduler = Sys.Scheduler as TestScheduler;
            timeSource = new ControlledTimeSource(date, testScheduler);
            timeSourceActor = Sys.ActorOf(TimeSimulatorActor.Props(timeSource), "time");
            timeSourceActorPath = timeSourceActor.Path.ToStringWithoutAddress();

            temperatureModel = new SimpleTemperatureModel();
            modelsActor = Sys.ActorOf<TemperatureModelActor>("models");
            modelsActor.Tell(temperatureModel);
            modelsActorPath = modelsActor.Path.ToStringWithoutAddress();
        }

        [Fact]
        public void Passes()
        {
            //In the test room
            var deviceDefinition = new TemperatureDeviceDefinition
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

            var initialStatus = new RoomStatus()
            {
                Name = "Test Room",
                Volume = 72
            };
            initialStatus.Parameters.Add(new ParameterValue(SensorType.Temperature, 25));

            roomActorRef = ActorOfAsTestActorRef(() => new RoomSimulatorActor(initialStatus, hole.Path), "room");
            roomActorRef.Tell(new AddTemperatureSensorMessage(
                timeSourceActor.Path.ToStringWithoutAddress(), 
                modelsActor.Path.ToStringWithoutAddress(),
                "Temp-sensor"));

            roomActorRef.Tell(new AddTemperatureActuatorMessage("Temp-controller", new []{ timeSourceActorPath, modelsActorPath }));
            roomActorRef.Tell(new AddDevice<ITemperatureDeviceDefinition>(deviceDefinition));

            //It's 7:30
            InitializeTimerTo(7, 30);
            //Initially it's 25'C in the room
            //There is a meeting at 10:30 we want to have 21'C by then
            SetMeeting(At(10, 30), At(12, 00),Temperature(21));

            //At 8:30 
            //Someone arranges an important meeting on 9:00 and wants 18'C
            MoveTimeTo(8, 30);
            SetMeeting(At(9, 00), At(10,00), Temperature(18));

            //At 9:00
            //We should have temperature around 18'C in the room
            MoveTimeTo(9, 00);
            TemperatureShouldBe(18);

            //At 10:00 (at the end of the important meeting)
            //The temperature should be still around 18'C
            MoveTimeTo(10, 00);
            TemperatureShouldBe(18);

            //At 10:30 
            //The temperature should be 21'C like we wanted before
            MoveTimeTo(10, 30);
            TemperatureShouldBe(21);

            //At 12:00 (After the last meeting)
            //TemperatureDevices should be turned off
            MoveTimeTo(12, 00);
            DevicesShouldBeTurnedOff();
        }

        private void InitializeTimerTo(int hours, int minutes)
        {
            timeSource.Reset(At(hours, minutes));
        }

        private void DevicesShouldBeTurnedOff()
        {
            var status = GetStatus();
            status.TemperatureDevices.ShouldAllBe(dev => dev.ActiveModeType == TemperatureModeType.Off || dev.ActiveModeType == TemperatureModeType.StandBy);
        }

        private void TemperatureShouldBe(double expectedTemperature)
        {
            Thread.Sleep(15000);
            var status = GetStatus();
            var temperature = Convert.ToDouble(status.Parameters[SensorType.Temperature].Value);
            temperature.ShouldBe(expectedTemperature, tolerance: 1);
        }

        private IRoomStatusMessage GetStatus()
        {
            return roomActorRef.UnderlyingActor.Status;
            //RoomActorRef.Tell(new RoomStatus.Request());
            //var status = ExpectMsg<RoomStatus>();
            //return status;
        }

        protected Expectation SetMeeting(Instant time, Instant endTime, params ParameterValue[] parameters)
        {
            var expectattion = new Expectation(time, endTime, parameters);
            roomActorRef.Tell(expectattion);
            return expectattion;
        }

        private void MoveTimeTo(int hours, int minutes)
        {
            int repetitions = (int)(At(hours, minutes) - timeSource.Now).TotalMinutes;

            for (int i = 0; i < repetitions; i++)
            {
                timeSourceActor.Tell(new AddMinutesMessage(1));
                Thread.Sleep(25);

                var T = Convert.ToDouble(roomActorRef.UnderlyingActor.Status.Parameters[SensorType.Temperature].Value);
                Debug.WriteLine($"at {timeSource.Now} T is {T}");
            }

            timeSource.Reset(At(hours, minutes));
        }

        private Instant At(int hours, int minutes = 0, int seconds = 0) => date
                                                                           + Duration.FromHours(hours)
                                                                           + Duration.FromMinutes(minutes)
                                                                           + Duration.FromSeconds(seconds);

        private static ParameterValue Temperature(double t) =>
            new ParameterValue(SensorType.Temperature, t);

        private static ParameterValuesCollection TemperatureOf(double t) =>
            new ParameterValuesCollection() { Temperature(t) };
    }
}
