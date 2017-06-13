using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Modules.RoomSimulator.Actors;
using OfficeHVAC.Modules.TemperatureSimulation;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace OfficeHVAC.TestScenarios
{
    public class Scenario1 : TestKit
    {
        private IActorRef Hole;

        private Instant Date;

        private IControlledTimeSource TimeSource;
        private ITemperatureModel TemperatureModel;
        private SimulatorModels Models;

        private IActorRef RoomActorRef;

        public Scenario1() : base(@"akka.scheduler.implementation = ""Akka.TestKit.TestScheduler, Akka.TestKit""")
        {
            Hole = ActorOf<BlackHoleActor>();

            Date = Instant.FromDateTimeUtc(DateTime.UtcNow.AddDays(1).Date);
            var testScheduler = this.Sys.Scheduler as TestScheduler;
            TimeSource = new ControlledTimeSource(Date, testScheduler);

            TemperatureModel = new SimpleTemperatureModel();

            Models = new SimulatorModels(TimeSource, TemperatureModel);
        }

        [Fact]
        public void Passes()
        {
            //In the test room
            var InitialStatus = new RoomStatus()
            {
                Name = "Test Room",
                Devices = new HashSet<IDevice>()
                {
                    new TemperatureDevice()
                    {
                        Id = "Turbo AC",
                        MaxPower = 2000,
                        Modes = new ModesCollection
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
                                PowerEfficiency = 0.95,
                                PowerConsumption = 0.3,
                                TemperatureRange = new Range<double>(-100, 100)
                            },
                            new TemperatureMode()
                            {
                                Name = "Turbo",
                                Type = TemperatureModeType.Turbo,
                                PowerEfficiency = 0.50,
                                PowerConsumption = 1.0,
                                TemperatureRange = new Range<double>(-100, 100)
                            }
                        }
                    }
                },
                Volume = 65
            };

            //It's 7:30
            TimeSource.Reset(At(7,30));
            //Initially it's 25'C in the room
            InitialStatus.Parameters.Add(new ParameterValue(SensorType.Temperature, 25));
            RoomActorRef = ActorOf(() => 
                new RoomSimulatorActor(InitialStatus, Hole.Path, 
                                       new TemperatureSimulatorFactory(TimeSource, TemperatureModel), 
                                       Models));
            //There is a meeting at 10:30 we want to have 21'C by then
            var firstMeeting = SetMeeting(At(10, 30), Temperature(21));

            //At 8:30 
            //Someone arranges an important meeting on 9:00 and wants 18'C
            SetTime(8,30);
            var importantMeeting = SetMeeting(At(9, 00), Temperature(18));

            //At 9:00
            //We should have temperature around 18'C in the room
            SetTime(9,00);
            TemperatureShouldBe(18);

            //At 11:00 (at the end of the important meeting)
            //The temperature should be still around 18'C
            SetTime(11,00);
            TemperatureShouldBe(18);

            //At 10:30 
            //The temperature should be 21'C like we wanted before
            SetTime(10,30);
            TemperatureShouldBe(21);

            //At 12:00 (After the last meeting)
            //Devices should be turned off
            SetTime(12,00);
            DevicesShouldBeTurnedOff();
        }

        private void DevicesShouldBeTurnedOff()
        {
            var status = GetStatus();
            status.Devices.ShouldAllBe(dev => dev.IsTurnedOn == false);
        }

        private void TemperatureShouldBe(double expectedTemperature)
        {
            Thread.Sleep(10000);
            var status = GetStatus();
            var temperature = Convert.ToDouble(status.Parameters[SensorType.Temperature].Value);
            temperature.ShouldBe(expectedTemperature, tolerance: 1);
        }

        private IRoomStatusMessage GetStatus()
        {
            RoomActorRef.Tell(new RoomStatus.Request());
            var status = ExpectMsg<RoomStatus>();
            return status;
        }

        private Requirements SetMeeting(Instant time, params ParameterValue[] parameters)
        {
            var pars = new ParameterValuesCollection();
            foreach (var parameter in parameters)
                pars.Add(parameter);

            var requirements = new Requirements(time, pars);
            
            RoomActorRef.Tell(requirements);

            return requirements;
        }

        private void SetTime(int hours, int minutes)
        {
            int repetitions = (int)(At(hours, minutes) - TimeSource.Now).TotalMinutes;

            for (int i = 0; i < repetitions; i++)
            {
                TimeSource.AddMinutes(1);
                Thread.Sleep(10);
            }

            TimeSource.Reset(At(hours, minutes));
        }

        private Instant At(int hours, int minutes = 0, int seconds = 0) => Date 
                                                                           + Duration.FromHours(hours)
                                                                           + Duration.FromMinutes(minutes)
                                                                           + Duration.FromSeconds(seconds);

        private static ParameterValue Temperature(double t) =>
            new ParameterValue(SensorType.Temperature, t);

        private static ParameterValuesCollection TemperatureOf(double t) =>
            new ParameterValuesCollection() { Temperature(t) };
    }
}
