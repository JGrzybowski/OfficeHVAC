﻿using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Messages
{
    public class AddDevice<TDeviceDefinition>
    {
        public readonly TDeviceDefinition Definition;

        public AddDevice(TDeviceDefinition definition)
        {
            Definition = definition;
        }
    }
}
