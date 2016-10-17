using System;

namespace OfficeHVAC.Models
{
    public interface ITimeSource
    {
        DateTime Time { get; }
    }
}