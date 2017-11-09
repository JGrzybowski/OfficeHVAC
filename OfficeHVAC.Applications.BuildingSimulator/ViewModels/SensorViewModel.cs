using System;
using System.Collections.ObjectModel;
using Akka.Actor;
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

        public IActorRef Actor { get; set; }
    }
    
    public class SensorViewModel<TParamValue> : SensorViewModel 
        where TParamValue : struct
    {
        protected TParamValue paramValue;
        public TParamValue ParamValue  
        {
            get => paramValue;
            set => SetProperty(ref paramValue, value);
        }
    }
}