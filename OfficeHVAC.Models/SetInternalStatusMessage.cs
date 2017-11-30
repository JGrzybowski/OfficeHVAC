namespace OfficeHVAC.Models {
    public class SetInternalStatusMessage<TInternalStatus>
    {
        public TInternalStatus Status { get; }
        public SetInternalStatusMessage(TInternalStatus status)
        {
            Status = status;
        }
    }
}