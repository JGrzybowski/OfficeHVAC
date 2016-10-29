using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.Configs;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomBridgeActor
{
    public class Constructor : TestKit
    {
        private readonly ViewModels.RoomViewModel _viewModel =
            Substitute.For<ViewModels.RoomViewModel>(
                Substitute.For<IRoomSimulatorActorPropsFactory>(),
                Substitute.For<IConfigBuilder>()
            );

        private readonly IActorRef _echoActor;

        public Constructor()
        {
            _echoActor = ActorOf(EchoActor.Props(this, false));
        }
    }
}
