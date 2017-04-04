using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Models.Devices
{
    public class ModesCollection : ObservableKeyedCollection<TemperatureModeType, ITemperatureMode>
    {
        protected override TemperatureModeType GetKeyForItem(ITemperatureMode item) => item.Type;

        public ModesCollection() { }

        public ModesCollection(IEnumerable<ITemperatureMode> modes)
        {
            if (modes.Select(m => m.Type).Distinct().Count() < modes.Count())
                throw new ArgumentException("When constyrructing ModesCollection all modes must have different ModeTypes.", nameof(TemperatureModeType));

            foreach (var mode in modes)
                Add(mode);
        }
    }
}
