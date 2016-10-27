using OfficeHVAC.Factories.ActorPaths;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Factories.Tests.ActorPaths
{
    public class RemoteActor
    {
        [Fact]
        public void should_construct_proper_central_system_address()
        {
            //Arrange
            var builder = new RemoteActorPathBuilder()
            {
                ServerAddress = "192.168.40.40",
                ServerPort = 8080,
                CompanyActorName = "company"
            };

            //Act
            var actorPath = builder.ActorPath();

            //Assert
            actorPath.ToString().ShouldBe("akka.tcp://HVACsystem@192.168.40.40:8080/user/company");
        }

        [Theory]
        [InlineData("localhost")]
        [InlineData("127.0.0.1")]
        public void should_skip_tcp_when_server_address_is_localhost(string localAddress)
        {
            //Arrange
            var builder = new RemoteActorPathBuilder()
            {
                ServerAddress = localAddress,
                ServerPort = 8080,
                CompanyActorName = "company"
            };

            //Act
            var actorPath = builder.ActorPath();

            //Assert
            actorPath.ToString().ShouldBe($"akka://HVACsystem@{localAddress}:8080/user/company");
        }

        [Fact]
        public void should_skip_port_when_port_number_is_empty()
        {
            //Arrange
            var builder = new RemoteActorPathBuilder()
            {
                ServerAddress = "192.168.40.40",
                ServerPort = null,
                CompanyActorName = "company"
            };

            //Act
            var actorPath = builder.ActorPath();

            //Assert
            actorPath.ToString().ShouldBe("akka.tcp://HVACsystem@192.168.40.40/user/company");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        public void should_skip_address_and_port_when_address_and_port_number_is_empty(string localAddress)
        {
            //Arrange
            var builder = new RemoteActorPathBuilder()
            {
                ServerAddress = localAddress,
                ServerPort = null,
                CompanyActorName = "company"
            };

            //Act
            var actorPath = builder.ActorPath();

            //Assert
            actorPath.ToString().ShouldBe("akka://HVACsystem/user/company");
        }
    }
}
