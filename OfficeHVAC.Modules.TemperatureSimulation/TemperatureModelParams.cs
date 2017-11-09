namespace OfficeHVAC.Modules.TemperatureSimulation 
{
    public struct TemperatureModelParams 
    {
        public double AirsSpecificHeat { get; }  //  W / kg*C*s
        public double AirsDensity { get; }       // kg / m^3
        
        public TemperatureModelParams(double airsSpecificHeat, double airsDensity)
        {
            AirsSpecificHeat = airsSpecificHeat;
            AirsDensity = airsDensity;
        }
    }
}