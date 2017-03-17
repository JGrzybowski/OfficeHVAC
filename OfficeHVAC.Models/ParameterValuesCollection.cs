using System.Collections.ObjectModel;

namespace OfficeHVAC.Models
{
    public class ParameterValuesCollection : KeyedCollection<SensorType, ParameterValue>
    {
        protected override SensorType GetKeyForItem(ParameterValue item) => item.ParameterType;

        public ParameterValuesCollection Clone()
        {
            var clone = new ParameterValuesCollection();
            foreach (var parameter in this)
                clone.Add(parameter);
            return clone;
        }
    }
}
