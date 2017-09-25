using Akka.Actor;
using OfficeHVAC.Applications.BuildingSimulator.ViewModels;

namespace OfficeHVAC.Applications.BuildingSimulator.Actors
{
    public class CompanyBridgeActor : ReceiveActor
    {
        public CompanyBridgeActor(IActorRef companyActor, CompanyViewModel viewModel)
        {
            CompanyActor = companyActor;
            ViewModel = viewModel;

            Receive<CreateRoomMessage>(msg => PushToActor(msg));
            Receive<RemoveRoomMessage>(msg => PushToActor(msg));
               
        }
        private IActorRef CompanyActor { get; set; }
        private CompanyViewModel ViewModel { get; set; }

        private void PushToActor(object msg)
        {
            CompanyActor.Forward(msg);
        }
    }
}