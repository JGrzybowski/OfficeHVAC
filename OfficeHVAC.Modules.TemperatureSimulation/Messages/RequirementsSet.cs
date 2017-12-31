using System;
using System.Collections.Generic;
using NodaTime;
using OfficeHVAC.Models;

namespace OfficeHVAC.Modules.TemperatureSimulation.Messages
{
    public class RequirementsSet<TParam, TActuatorDefinition, TParamModel>
    {
        public TParam CurrentParamValue { get; }
        public Instant InitialTime { get; }
        public IEnumerable<Requirement<TParam>> Requirements { get; }
        public IEnumerable<TActuatorDefinition> Actuators { get; }
        public TParamModel Model { get; }
        public IRoomStatusMessage RoomStatus { get; }

        public RequirementsSet(TParam currentParamValue, Instant initialTime,
            IEnumerable<Requirement<TParam>> requirements, IEnumerable<TActuatorDefinition> actuators, 
            TParamModel model, IRoomStatusMessage roomStatus)
        {
            CurrentParamValue = currentParamValue;
            InitialTime = initialTime;
            Requirements = requirements;
            Actuators = actuators;
            Model = model;
            RoomStatus = roomStatus;
        }
    }
}