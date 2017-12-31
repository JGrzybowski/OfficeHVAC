using Akka.TestKit.Xunit2;
using NodaTime;
using NSubstitute;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System.Collections.Generic;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.TemperatureControllerActor
{
    public class TemperatureControllerActorTest : TestKit
    {
        protected static ITemperatureModel GenerateModelFake(double temperatureChange = 0, Duration neededTime = default(Duration))
        {
            var fake = Substitute.For<ITemperatureModel>();
            fake.CalculateChange(Arg.Any<double>(), Arg.Any<IEnumerable<ITemperatureDeviceStatus>>(), Arg.Any<Duration>(), Arg.Any<double>())
                .ReturnsForAnyArgs(temperatureChange);
            fake.CalculateNeededTime(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<IEnumerable<ITemperatureDeviceDefinition>>(), Arg.Any<TemperatureModeType>(), Arg.Any<double>())
                .ReturnsForAnyArgs(neededTime);
            return fake;
        }
    }
}
