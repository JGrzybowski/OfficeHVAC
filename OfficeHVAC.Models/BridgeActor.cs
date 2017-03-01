using Akka.Actor;

namespace OfficeHVAC.Models
{
    public abstract class BridgeActor<TViewModel> : ReceiveActor
    {
        protected TViewModel ViewModel { get; }

        protected BridgeActor(TViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
