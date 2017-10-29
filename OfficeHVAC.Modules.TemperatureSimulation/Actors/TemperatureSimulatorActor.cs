using Akka.Actor;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TimeSimulation.Messages;
using System;
using OfficeHVAC.Modules.TemperatureSimulation.Messages;

namespace OfficeHVAC.Modules.TemperatureSimulation.Actors
{
    public class TemperatureSimulatorActor : ReceiveActor
    {
        private IRoomStatusMessage roomStatus;

        //private ICancelable Scheduler { get; set; }
        private ITemperatureSimulator temperatureSimulator { get; }

        public TemperatureSimulatorActor(ITemperatureSimulator temperatureSimulator, string temperatureParamsActorPath)
        {
            this.temperatureSimulator = temperatureSimulator;
            Become(AwaitingInitialModel);
            Context.System.ActorSelection(temperatureParamsActorPath).Tell(new TemperatureModelRequest());
        }

        private void AwaitingInitialModel()
        {
            Receive<ITemperatureModel>(model =>
                {
                    temperatureSimulator.ReplaceTemperatureModel(model);
                    Become(Processing);
                }
            );
        }

        private void Processing()
        {
            Receive<ParameterValue.Request>(
                msg => Sender.Tell(new ParameterValue(SensorType.Temperature, this.temperatureSimulator.Temperature)),
                msg => msg.ParameterType == SensorType.Temperature
            );

            Receive<TimeChangedMessage>(
                msg => this.temperatureSimulator.ChangeTemperature(roomStatus, msg.TimeDelta)
            );

            Receive<IRoomStatusMessage>(msg => roomStatus = msg);
            
            Receive<ITemperatureModel>(model => this.temperatureSimulator.ReplaceTemperatureModel(model));
        }
        
        protected override void PreStart()
        {
            //Scheduler = Context.System
            //    .Scheduler
            //    .ScheduleTellRepeatedlyCancelable(
            //        TimeSpan.FromMilliseconds(1000),
            //        TimeSpan.FromMilliseconds(15000),
            //        Self,
            //        new ParameterValue.Request(SensorType.Temperature),
            //        Context.Parent);

            base.PreStart();
        }

        public static Props Props(RoomStatus initialStatus, string timeActorPath, string tempParamsActorPath)
        {
            double initialTemperature = 0;
            if (initialStatus.Parameters.Contains(SensorType.Temperature))
                initialTemperature = Convert.ToDouble(initialStatus.Parameters[SensorType.Temperature].Value);

            var props = Akka.Actor.Props.Create(
                () => new TemperatureSimulatorActor(new TemperatureSimulator(initialTemperature, null), tempParamsActorPath)
            );

            return props;
        }
    }
}