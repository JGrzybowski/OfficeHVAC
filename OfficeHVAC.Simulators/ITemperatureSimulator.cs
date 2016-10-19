namespace OfficeHVAC.Simulators
{
    public interface ITemperatureSimulator : IParameterSimulator
    {
        float Temperature { get; set; }
    }
}