using System.Collections;
using System.Collections.Generic;
using OfficeHVAC.Models;

namespace OfficeHVAC.Simulators
{
    public interface ITemperatureSimulator : IParameterSimulator
    {
        float Temperature { get; set; }
    }
}