using Akka.Configuration;
using OfficeHVAC.Factories.Configs;
using Shouldly;
using Xunit;
using Xunit.Sdk;

namespace OfficeHVAC.Actors.Tests
{
    public class Remote
    {
        [Fact]
        public void should_use_akka_remote()
        {
            //Arrange
            var builder = new RemoteConfigBuilder()
            {
                Port = 8080
            };

            //Act
            Config config = builder.Config();

            //Assert
            config.GetString("akka.actor.provider").ShouldBe("Akka.Remote.RemoteActorRefProvider, Akka.Remote");
        }

        [Fact]
        public void should_use_specified_port()
        {
            //Arrange
            var builder = new RemoteConfigBuilder()
            {
                Port = 8080
            };

            //Act
            Config config = builder.Config();

            //Assert
            config.GetInt("akka.remote.helios.tcp.port").ShouldBe(8080);
        }
    }
}
