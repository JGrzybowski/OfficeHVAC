namespace OfficeHVAC.Applications.BuildingSimulator.Actors {
    public class ChangeNameMessage {
        public ChangeNameMessage(string newValue)
        {
            NewValue = newValue;
        }
        public string NewValue { get; }
    }
}