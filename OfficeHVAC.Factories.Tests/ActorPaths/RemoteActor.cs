using OfficeHVAC.Factories.ActorPaths;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Factories.Tests.ActorPaths
{
    public class RemoteActor
    {
        private const string TestSystemName = "ActorSystem";

        [Fact]
        public void should_construct_proper_central_system_address()
        {
            //Arrange
            var builder = new RemoteActorPathBuilder()
            {
                SystemName = TestSystemName,
                ServerAddress = "192.168.40.40",
                ServerPort = 8080,
                CompanyActorName = "company"
            };

            //Act
            var actorPath = builder.ActorPath();

            //Assert
            actorPath.ToString().ShouldBe($"akka.tcp://{TestSystemName}@192.168.40.40:8080/user/company");
        }

        [Theory]
        [InlineData("localhost")]
        [InlineData("127.0.0.1")]
        [InlineData("some.web.address")]
        public void should_always_use_tcp(string address)
        {
            //Arrange
            var builder = new RemoteActorPathBuilder()
            {
                SystemName = TestSystemName,
                ServerAddress = address,
                ServerPort = 8080,
                CompanyActorName = "company"
            };

            //Act
            var actorPath = builder.ActorPath();

            //Assert
            actorPath.ToString().ShouldBe($"akka.tcp://{TestSystemName}@{address}:8080/user/company");
        }

        [Fact]
        public void should_skip_port_when_port_number_is_empty()
        {
            //Arrange
            var builder = new RemoteActorPathBuilder()
            {
                SystemName = TestSystemName,
                ServerAddress = "192.168.40.40",
                ServerPort = null,
                CompanyActorName = "company"
            };

            //Act
            var actorPath = builder.ActorPath();

            //Assert
            actorPath.ToString().ShouldBe($"akka.tcp://{TestSystemName}@192.168.40.40/user/company");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        public void should_skip_address_and_port_when_address_and_port_number_is_empty(string localAddress)
        {
            //Arrange
            var builder = new RemoteActorPathBuilder()
            {
                SystemName = TestSystemName,
                ServerAddress = localAddress,
                ServerPort = null,
                CompanyActorName = "company"
            };

            //Act
            var actorPath = builder.ActorPath();

            //Assert
            actorPath.ToString().ShouldBe($"akka.tcp://{TestSystemName}/user/company");
        }
    }
}
