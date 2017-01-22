namespace OfficeHVAC.Models.Devices
{
    public interface IMode
    {
        string Name { get; set; }

        double PowerConsumption { get; set; }

        double PowerEfficiency { get; set; }

        double CalculateEffectivePower(double maxPower);
    }
}
