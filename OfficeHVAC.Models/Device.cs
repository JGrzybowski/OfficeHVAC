using System;

namespace OfficeHVAC.Models
{
    public class Device : IDevice
    {
        public bool IsTurnedOn
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public byte UsedPower
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void TurnOff()
        {
            throw new NotImplementedException();
        }
    }
}
