using OfficeHVAC.Models;

namespace OfficeHVAC.Factories.TimeSources
{
    public interface ITimeSourceFactory
    {
        ITimeSource TimeSource();
    }
}