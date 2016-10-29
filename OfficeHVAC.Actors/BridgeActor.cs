using Akka.Actor;

namespace OfficeHVAC.Actors
{
    public abstract class BridgeActor<TViewModel> : ReceiveActor
    {
        private TViewModel ViewModel { get; }

        protected BridgeActor(TViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }
    }
}
