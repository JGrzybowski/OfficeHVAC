using Xunit;
using Xunit.Abstractions;

namespace OfficeHVAC.TestScenarios
{
    public class ExtendingMeeting : ScenarioBase
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

            //At 9:00
            MoveTimeTo(9, 00);
            TemperatureShouldBe(18);

            //At 10:45
            MoveTimeTo(10, 45);
            ExtendMeeting(At(9,00), At(11,30));

            //At 11:00 
            MoveTimeTo(11, 00);
            TemperatureShouldBe(18);

            //At 11:30 (After the last meeting)
            //TemperatureDevices should be turned off
            MoveTimeTo(11, 30);
            TemperatureShouldBe(18);

            MoveTimeTo(11, 35);
            DevicesShouldBeTurnedOff();
        }

        public ExtendingMeeting(ITestOutputHelper output) : base(output)
        {
        }
    }
}
