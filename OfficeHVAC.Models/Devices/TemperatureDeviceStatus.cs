﻿using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Models.Devices
{
    public interface ITemperatureDeviceStatus {
        string Id { get; }
        int MaxPower { get; }
        IEnumerable<ITemperatureMode> Modes { get; }
        TemperatureModeType ActiveModeType { get; }
        ITemperatureMode ActiveMode { get; }
        double EffectivePower { get; }
        double GetDesiredTemperature();
    }

    public class TemperatureDeviceStatus : ITemperatureDeviceStatus
    {
        public TemperatureDeviceStatus(string id, int maxPower, IEnumerable<ITemperatureMode> modes, TemperatureModeType activeModeType, double desiredTemperature)
        {
            Id = id;
            MaxPower = maxPower;
            Modes = modes;
            ActiveModeType = activeModeType;
            this.desiredTemperature = desiredTemperature;
        }
        
        public string Id { get; }
        public int MaxPower { get; }
        public IEnumerable<ITemperatureMode> Modes { get; }
        public TemperatureModeType ActiveModeType { get; }
        public ITemperatureMode ActiveMode => Modes.Single(m => m.Type == ActiveModeType);

        public double EffectivePower => ActiveMode.CalculateEffectivePower(MaxPower);

        private readonly double desiredTemperature;

        public double GetDesiredTemperature()
        {
            return desiredTemperature;
        }
    }
}