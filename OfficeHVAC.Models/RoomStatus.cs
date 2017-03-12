using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Models
{
    public class RoomStatus : IRoomStatusMessage, IToMessage<IRoomStatusMessage>
    {
        public RoomInfo RoomInfo { get; set; }

        public IEnumerable<ISensorActorRef> Sensors { get; private set; }

        public IEnumerable<IParameterValueMessage> ParameterValues { get; private set; }

        public IRoomStatusMessage ToMessage() => this.Clone() as IRoomStatusMessage;
        
        public object Clone()
        {
            return new RoomStatus()
            {
                RoomInfo = this.RoomInfo.Clone() as RoomInfo,
                ParameterValues = this.ParameterValues.Select(param => param.Clone() as IParameterValueMessage).ToList(),
                Sensors = this.Sensors.Select(param => param.Clone() as ISensorActorRef).ToList()
            };
        }

        public class Request { }
    }

    public interface IRoomStatusMessage : ICloneable
    {
        RoomInfo RoomInfo { get; }

        IEnumerable<IParameterValueMessage> ParameterValues { get; }
    }
}
