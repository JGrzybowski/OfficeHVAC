using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using OfficeHVAC.Applications.RoomSimulator.ViewModels;

namespace OfficeHVAC.Applications.RoomSimulator.Factories
{
    public class BridgeRoomActorPropsFactory: IBridgeRoomActorPropsFactory
    {
        public IRoomSimulatorActorPropsFactory RoomSimulatorActorPropsFactory { get; }
        public IRoomViewModel ViewModel { get; }

        public BridgeRoomActorPropsFactory(IRoomSimulatorActorPropsFactory roomSimulatorActorPropsFactory, IRoomViewModel viewModel)
        {
            RoomSimulatorActorPropsFactory = roomSimulatorActorPropsFactory;
            ViewModel = viewModel;
        }

        public Props Props()
        {
            throw new NotImplementedException();
        }
    }
}
