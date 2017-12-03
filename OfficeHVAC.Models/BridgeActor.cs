using Akka.Actor;
using System;

namespace OfficeHVAC.Models
{
    public abstract class BridgeActor<TViewModel> : ReceiveActor
    {
        protected TViewModel ViewModel { get; }
        protected IActorRef Actor { get; }

        protected BridgeActor(TViewModel viewModel, IActorRef actorRef)
        {
            ViewModel = viewModel;
            Actor = actorRef;
        }
    }
}
