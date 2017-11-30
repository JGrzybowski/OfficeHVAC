using System;
using System.Collections.ObjectModel;
using Akka.Actor;
using NodaTime;
using OfficeHVAC.Models;
using Prism.Mvvm;

namespace OfficeHVAC.Applications.BuildingSimulator.ViewModels {
    public class SensorViewModel : BindableBase, ITreeElement 
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        protected string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        
        public ObservableCollection<ITreeElement> SubItems { get; } = new ObservableCollection<ITreeElement>();

        protected SensorType sensorType = SensorType.Unknown;
        public SensorType SensorType
        {
            get => sensorType;
            protected set => SetProperty(ref sensorType, value);
        }
        
        protected Instant timestamp;
        public Instant Timestamp
        {
            get => timestamp;
            set => SetProperty(ref timestamp, value);
        }

        public IActorRef Actor { get; set; }
    }
    
    public abstract class SensorViewModel<TSensorStatus, TParamValue> : SensorViewModel 
        where TParamValue : struct
    {
        protected TParamValue paramValue;
        public TParamValue ParamValue  
        {
            get => paramValue;
            set => SetProperty(ref paramValue, value);
        }
        public void PushParam(TParamValue value) => SetProperty(ref paramValue, value, nameof(ParamValue));

        public abstract void PushStatus(TSensorStatus status);
    }
}