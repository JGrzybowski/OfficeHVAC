using System;
using System.Diagnostics;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using Xunit;
using Xunit.Abstractions;

namespace OfficeHVAC.TestScenarios
{
    public class DealingWithTemperatureAnomalies : ScenarioBase
    {
        public DealingWithTemperatureAnomalies(ITestOutputHelper output) : base(output)
        {
            initialStatus.Parameters.Remove(SensorType.Temperature);
            initialStatus.Parameters.Add(Temperature(20));
        }

        [Fact]
        public void Passes()
        {
            InitializeRoom();
            //It's 7:30
            InitializeTimerTo(7, 30);
            //Initially it's 20'C in the room
            //There is a meeting at 09:00 we want to have 23'C by then
            SetMeeting(At(09, 00), At(11, 00), Temperature(23));

            //At 8:30 
            //Someone opens the windows at 8:30 causing the temperature to drop to 18'C
            MoveTimeTo(8, 15);
            RoomActorRef.Tell(new SetTemperature(18));
            MoveTimeTo(8,16);
            TemperatureShouldBe(18);
            
            //At 9:00
            MoveTimeTo(9, 00);
            TemperatureShouldBe(23);

            //At 11:00 
            //TemperatureDevices should be turned off
            MoveTimeTo(11, 00);
            TemperatureShouldBe(23);

            MoveTimeTo(11, 05);
            DevicesShouldBeTurnedOff();
        }
    }
}
