﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace OfficeHVAC.Models
{
    public class SimulatorModels : ISimulatorModels
    {
        public IActorRef TimeSource => timeSource;
        private IActorRef timeSource;

        public ITemperatureModel TemperatureModel => temperatureModel;
        private ITemperatureModel temperatureModel;

        public SimulatorModels(IActorRef timeSource, ITemperatureModel temperatureModel)
        {
            this.timeSource = timeSource;
            this.temperatureModel = temperatureModel;
        }
    }
}
