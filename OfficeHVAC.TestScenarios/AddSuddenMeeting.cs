using Xunit;
using Xunit.Abstractions;

namespace OfficeHVAC.TestScenarios
{
    public class AddSuddenMeeting : ScenarioBase
    {
        [Fact]
        public void Passes()
        {
            InitializeRoom();
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
            TemperatureShouldBe(21);

            MoveTimeTo(12,10);
            DevicesShouldBeTurnedOff();
        }

        public AddSuddenMeeting(ITestOutputHelper output) : base(output)
        {
        }
    }
}
