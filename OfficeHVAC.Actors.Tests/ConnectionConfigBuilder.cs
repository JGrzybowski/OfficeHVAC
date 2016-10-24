using Shouldly;
using Xunit;
using Xunit.Sdk;

namespace OfficeHVAC.Actors.Tests
{
    public class ConnectionConfigBuilder
    {
        [Fact]
        public void should_construct_proper_central_system_address()
        {
            //Arrange
            var builder = new ConnectionConfig.Builder()
            {
                ListeningPort = 8080,
                ServerAddress = "192.168.40.40",
                ServerPort = 8080,
                CompanyActorName = "company"
            };

            //Act
            IConnectionConfig config = builder.Build();

            //Assert
            config.CompanyActorPath.ToString().ShouldBe("akka.tcp://HVACsystem@192.168.40.40:8080/user/company");
        }

        [Theory]
        [InlineData("localhost")]
        [InlineData("127.0.0.1")]
        public void should_skip_tcp_when_server_address_is_localhost(string localAddress)
        {
            //Arrange
            var builder = new ConnectionConfig.Builder()
            {
                ServerAddress = localAddress,
                ListeningPort = 8080,
                ServerPort = 8080,
                CompanyActorName = "company"
            };

            //Act
            IConnectionConfig config = builder.Build();

            //Assert
            config.CompanyActorPath.ToString().ShouldBe($"akka://HVACsystem@{localAddress}:8080/user/company");
        }

        [Fact]
        public void should_skip_port_when_port_number_is_empty()
        {
            //Arrange
            var builder = new ConnectionConfig.Builder()
            {
                ServerAddress = "192.168.40.40",
                ServerPort = null,
                ListeningPort = 8080,
                CompanyActorName = "company"
            };

            //Act
            IConnectionConfig config = builder.Build();

            //Assert
            config.CompanyActorPath.ToString().ShouldBe("akka.tcp://HVACsystem@192.168.40.40/user/company");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        public void should_skip_address_and_port_when_address_and_port_number_is_empty(string localAddress)
        {
            //Arrange
            var builder = new ConnectionConfig.Builder
            {
                ServerAddress =  localAddress,
                ServerPort = null,
                ListeningPort = 8080,
                CompanyActorName = "company"
            };

            //Act
            IConnectionConfig config = builder.Build();

            //Assert
            config.CompanyActorPath.ToString().ShouldBe("akka://HVACsystem/user/company");
        }
    }
}
