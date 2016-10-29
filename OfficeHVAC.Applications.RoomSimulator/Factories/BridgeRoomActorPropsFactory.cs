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
        public RoomViewModel ViewModel { get; set; }

        public BridgeRoomActorPropsFactory(IRoomSimulatorActorPropsFactory roomSimulatorActorPropsFactory, RoomViewModel viewModel)
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
