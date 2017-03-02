using Akka.Actor;
using OfficeHVAC.Messages;
using OfficeHVAC.Modules.TemperatureSimulation;
using System;
using System.Collections.Generic;

namespace OfficeHVAC.Modules.RoomSimulator.Actors
{
    public class RoomSimulatorActor : ReceiveActor
    {
        private string RoomName { get; }

        private ICancelable Scheduler { get; set; }

        private ITemperatureSimulator TemperatureSimulator { get; }

        private HashSet<IActorRef> Subscribers { get; } = new HashSet<IActorRef>();

        private ActorPath CompanySupervisorActorPath { get; }

        public RoomSimulatorActor(string roomName, ITemperatureSimulator temperatureSimulator, ActorPath companySupervisorActorPath)
        {
            this.RoomName = roomName;
            this.TemperatureSimulator = temperatureSimulator;
            this.CompanySupervisorActorPath = companySupervisorActorPath;

            this.Receive<SubscribeMessage>(message =>
            {
                this.Subscribers.Add(Sender);
                Sender.Tell(GenerateRoomStatus());
            });

            this.Receive<UnsubscribeMessage>(message =>
            {
                this.Subscribers.Remove(Sender);
            });

            this.Receive<RoomStatusRequest>(message =>
            {
                var status = GenerateRoomStatus();
                foreach (var subscriber in this.Subscribers)
                    subscriber.Tell(status, Self);
            });

            this.Receive<ChangeTemperature>(message =>
            {
                TemperatureSimulator.Temperature += message.DeltaT;
            });

            //this.Receive<SetDesiredTemperature>(message => {
            //    foreach (ITemperatureDevice device in TemperatureSimulator.Devices)
            //        device.DesiredTemperature = message.DesiredTemperature;
            //});

            //this.Receive<SetDesiredMode>(message =>
            //{
            //    foreach (ITemperatureDevice device in TemperatureSimulator.Devices)
            //        device.SetActiveModeByName = message.DesiredMode;
            //});
        }

        private RoomStatusMessage GenerateRoomStatus()
        {
            return new RoomStatusMessage(RoomName, TemperatureSimulator.Temperature);
        }

        protected override void PreStart()
        {
            var selection = Context.System.ActorSelection(this.CompanySupervisorActorPath.ToString());
            selection.Tell(new RoomAvaliabilityMessage(Self));

            Scheduler = Context.System
                .Scheduler
                .ScheduleTellRepeatedlyCancelable(
                    TimeSpan.FromMilliseconds(1000),
                    TimeSpan.FromMilliseconds(1000),
                    Self,
                    new RoomStatusRequest(), 
                    Self);
        }
    }
}
