namespace OfficeHVAC.Simulators
{
    public interface ITemperatureSimulator : IParameterSimulator
    {
        double Temperature { get; set; }
    }
}