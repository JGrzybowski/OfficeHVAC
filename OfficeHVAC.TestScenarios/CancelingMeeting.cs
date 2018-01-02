using Xunit;
using Xunit.Abstractions;

namespace OfficeHVAC.TestScenarios
{
    public class CancelingMeeting : ScenarioBase
    {
        [Fact]
        public void Passes()
        {
            InitializeRoom();

            //It's 7:30
            InitializeTimerTo(7, 30);
            //Initially it's 25'C in the room
            //There is a meeting at 09:00 we want to have 18'C by then
            //There is a meeting at 10:30 we want to have 21'C by then
            SetMeeting(At(09, 00), At(10, 00), Temperature(18));
            SetMeeting(At(10, 30), At(12, 00), Temperature(21));

            //At 8:30 
            //Someone canceles the meeting at 9:00
            MoveTimeTo(8, 30);
            CancelMeeting(At(09, 00));

            //At 9:00
            MoveTimeTo(9, 00);
            DevicesShouldBeTurnedOff();

            //At 10:30 
            //The temperature should be 21'C like we wanted before
            MoveTimeTo(10, 30);
            TemperatureShouldBe(21);

            //At 12:00 (After the last meeting)
            //TemperatureDevices should be turned off
            MoveTimeTo(12, 00);
            TemperatureShouldBe(21);

            MoveTimeTo(12, 05);
            DevicesShouldBeTurnedOff();
        }

        public CancelingMeeting(ITestOutputHelper output) : base(output)
        {
        }
    }
}
