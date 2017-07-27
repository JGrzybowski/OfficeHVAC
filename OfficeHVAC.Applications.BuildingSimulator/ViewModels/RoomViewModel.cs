using OfficeHVAC.Models;
using Prism.Mvvm;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels
{
    public class RoomViewModel : BindableBase
    {
        private RoomStatus status;
        public RoomStatus Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }


    }
}