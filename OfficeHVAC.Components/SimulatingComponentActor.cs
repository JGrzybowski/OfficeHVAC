﻿using NodaTime;
using OfficeHVAC.Messages;
using OfficeHVAC.Models;
using System.Collections.Generic;

namespace OfficeHVAC.Components
{
    public abstract class SimulatingComponentActor<TInternalStatus, TParameter> : CyclicComponentActor<TInternalStatus, TParameter> 
        where TInternalStatus : ComponentStatus<TParameter>
    {
        protected IRoomStatusMessage RoomStatus;
        protected bool ReceivedInitialRoomStatus => RoomStatus != null;

        protected override bool ReceivedInitialData() =>
            TimeStampInitialized && ReceivedInitialRoomStatus;
        
        protected SimulatingComponentActor(IEnumerable<string> subscriptionSources) : base(subscriptionSources) { }

        protected override void Uninitialized()
        {
            Receive<IRoomStatusMessage>(msg =>
            {
                UpdateRoomStatus(msg);
                if(!ReceivedInitialRoomStatus)
                    InitializeFromRoomStatus(msg);
                InformAboutInternalState();
                if (ReceivedInitialData())
                    Become(Initialized);
            });

            base.Uninitialized();
        }

        protected override void Initialized()
        {
            Receive<SetParameterValueMessage<TParameter>>(msg =>
            {
                SetParameterValue(msg.Value);
                InformAboutInternalState();
            });

            Receive<IRoomStatusMessage>(
                msg => UpdateRoomStatus(msg), 
                msg => RoomStatus.TimeStamp < msg.TimeStamp);
            
            base.Initialized();
        }

        protected virtual void InitializeFromRoomStatus(IRoomStatusMessage roomStatus) { }

        protected virtual void SetParameterValue(TParameter value)
        {
            ParameterValue = value;
        }
        
        protected virtual void UpdateRoomStatus(IRoomStatusMessage roomStatus)
        {
            RoomStatus = roomStatus;
        }
    }

    public class SimulatingComponentStatus<TParameter> : ComponentStatus<TParameter>
    {
        public SimulatingComponentStatus(string id, TParameter parameter, Instant timestamp, Duration theresholdBuffer) 
        : base(id, parameter, timestamp)
        {
            TheresholdBuffer = theresholdBuffer;
        }
    
        public Duration TheresholdBuffer { get; }
    }
}