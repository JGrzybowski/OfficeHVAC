namespace OfficeHVAC.Applications.BuildingSimulator.Actors {
    public class UpdateCompanyMessage
    {
        public string Name { get; }
        
        public UpdateCompanyMessage(string name)
        {
            Name = name;
        }
    }
}