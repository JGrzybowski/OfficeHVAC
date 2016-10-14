using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace OfficeHVAC.Models
{
    public class Device : IDevice
    {
        public bool IsTurnedOn => UsedPower != 0;

        private byte usedPower = 0;
        public byte UsedPower
        {
            get { return usedPower; }

            set
            {
                if (value > 100)
                    throw new ArgumentOutOfRangeException(nameof(value));
                usedPower = value;
            }
        }

        public void TurnOff()
        {
            usedPower = 0;
        }
    }
}
