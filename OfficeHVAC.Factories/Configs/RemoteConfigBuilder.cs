﻿using Akka.Configuration;
using Prism.Mvvm;

namespace OfficeHVAC.Factories.Configs
{
    public class RemoteConfigBuilder : BindableBase, IConfigBuilder
    {
        private int? port;
        public int? Port
        {
            get { return port; }
            set { SetProperty(ref port, value); }
        }
        
        public Config Config()
        {
            throw new System.NotImplementedException();
        }
    }
}
