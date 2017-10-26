using System;
using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.RoomSimulator.ViewModels;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomBridgeActor : BridgeActor<IRoomViewModel>
    {
        public const string RoomActorName = "room";

        private readonly Props _roomActorProps;
        private IActorRef _roomActorRef;
        
        public RoomBridgeActor(IRoomViewModel viewModel, Props roomActorProps) : base(viewModel)
        {
            _roomActorProps = roomActorProps;
            _roomActorRef = Context.ActorOf(_roomActorProps, RoomActorName);
            
            Receive<SetTemperature>(msg => _roomActorRef.Tell(msg));
            Receive<ChangeTemperature>(msg => _roomActorRef.Tell(msg));
            Receive<IRoomStatusMessage>(msg => UpdateViewModel(msg));

            _roomActorRef.Tell(new SubscribeMessage(Self));
        }

        private void UpdateViewModel(IRoomStatusMessage msg)
        {
            if (msg.Parameters.Contains(SensorType.Temperature))
                ViewModel.Temperature = Convert.ToDouble(msg.Parameters[SensorType.Temperature].Value);
        }
    }
}
