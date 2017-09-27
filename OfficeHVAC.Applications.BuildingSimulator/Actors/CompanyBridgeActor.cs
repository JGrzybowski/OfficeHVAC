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

            Receive<CreateRoomMessage>(msg => PassToActor(msg));
            Receive<RemoveRoomMessage>(msg => PassToActor(msg));
            Receive<ChangeNameMessage>(msg => PushToActor(msg));
            Receive<UpdateCompanyMessage>(msg => UpdateViewModel(msg));
        }
        private IActorRef CompanyActor { get; set; }
        private CompanyViewModel ViewModel { get; set; }

        private void PassToActor(object msg) => CompanyActor.Forward(msg);
        private void PushToActor(object msg) => CompanyActor.Tell(msg);

        private void UpdateViewModel(UpdateCompanyMessage msg)
        {
            ViewModel.PushName(msg.Name);
        }
    }
}