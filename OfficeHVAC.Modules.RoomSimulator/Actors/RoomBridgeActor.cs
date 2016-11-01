﻿using Akka.Actor;
using OfficeHVAC.Actors;
using OfficeHVAC.Messages;
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
            Receive<RoomStatusMessage>(msg => UpdateViewModel(msg));

            _roomActorRef.Tell(new SubscribeMessage(Self));
        }

        private void UpdateViewModel(RoomStatusMessage msg)
        {
            ViewModel.Temperature = msg.Temperature;
        }
    }
}