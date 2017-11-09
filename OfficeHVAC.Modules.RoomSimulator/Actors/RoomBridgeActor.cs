using System;
using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Subscription;
using OfficeHVAC.Modules.RoomSimulator.ViewModels;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomBridgeActor : BridgeActor<IRoomViewModel>
    {
        public const string RoomActorName = "room";

        private readonly Props roomActorProps;
        private IActorRef roomActorRef;
        
        public RoomBridgeActor(IRoomViewModel viewModel, Props roomActorProps) : base(viewModel)
        {
            this.roomActorProps = roomActorProps;
            roomActorRef = Context.ActorOf(this.roomActorProps, RoomActorName);
            
            Receive<SetTemperature>(msg => roomActorRef.Tell(msg));
            Receive<ChangeTemperature>(msg => roomActorRef.Tell(msg));
            Receive<IRoomStatusMessage>(msg => UpdateViewModel(msg));

            Receive<AddTemperatureSensorMessage>(msg => roomActorRef.Forward(msg));
            Receive<RemoveSensorMessage>(msg => roomActorRef.Forward(msg));

            roomActorRef.Tell(new SubscribeMessage(Self));
        }

        private void UpdateViewModel(IRoomStatusMessage msg)
        {
            if (msg.Parameters.Contains(SensorType.Temperature))
                ViewModel.Temperature = Convert.ToDouble(msg.Parameters[SensorType.Temperature].Value);
        }
    }
}
