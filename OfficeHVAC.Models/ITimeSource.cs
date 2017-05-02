using NodaTime;

namespace OfficeHVAC.Models
{
    public interface ITimeSource
    {
        Instant Now { get; }
    }
}