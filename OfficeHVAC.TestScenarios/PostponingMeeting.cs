using Xunit;
using Xunit.Abstractions;

namespace OfficeHVAC.TestScenarios
{
    public class PostponingMeeting : ScenarioBase
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
            SetMeeting(At(09, 00), At(12, 00), Temperature(18));

            //At 8:30 
            //Someone canceles the meeting at 9:00
            MoveTimeTo(8, 30);
            PostponeMeeting(At(09, 00), At(11,00), At(14,00));

            //At 9:00
            MoveTimeTo(9, 00);
            DevicesShouldBeTurnedOff();

            //At 11:00 
            //The temperature should be 18'C like we wanted before
            MoveTimeTo(11, 00);
            TemperatureShouldBe(18);

            //At 14:00 (After the last meeting)
            //TemperatureDevices should be turned off
            MoveTimeTo(14, 00);
            TemperatureShouldBe(18);

            MoveTimeTo(14, 05);
            DevicesShouldBeTurnedOff();
        }

        public PostponingMeeting(ITestOutputHelper output) : base(output)
        {
        }
    }
}
