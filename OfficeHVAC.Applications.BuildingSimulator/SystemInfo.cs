﻿using OfficeHVAC.Applications.BuildingSimulator.ViewModels;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;

namespace OfficeHVAC.Applications.BuildingSimulator
{
    public static class SystemInfo
    {
        public const string SystemName = "OfficeHVAC";
        public const string TimeSimulatorActorName = TimeControlViewModel.TimeSimulatorActorName;
        public const string TempSimulatorModelActorName = TemperatureSimulatorParamsViewModel.TempSimulatorModelActorName;
        public static readonly string TimeSimulatorActorPath = $"{SystemName}/user/{TimeSimulatorActorName}";
        public static readonly string TempSimulatorModelActorPath = $"{SystemName}/user/{TimeSimulatorActorName}";
    }
}
