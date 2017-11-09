using System.Text;
using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.ServerSimulator.ViewModels;

namespace OfficeHVAC.Modules.ServerSimulator
{
    public class ServerBridgeActor : BridgeActor<ServerLogViewModel>
    {
        public IActorRef ServerActor { get; set; }

        public ServerBridgeActor(ServerLogViewModel viewModel, Props serverActorProps) : base(viewModel)
        {
            ServerActor = Context.ActorOf(serverActorProps, "Logger");

            Receive<RoomAvaliabilityMessage>(msg => ViewModel.Log($"Room {Sender.Path} is avaliable."));

            Receive<IRoomStatusMessage>(msg =>
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Room {msg.Name}");
                foreach (var paramValue in msg.Parameters)
                    sb.AppendLine($"{paramValue.ToString()} - {paramValue.Value.ToString()}");

                ViewModel.Log(sb.ToString());
            });
        }
    }
}
