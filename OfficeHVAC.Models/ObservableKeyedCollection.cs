using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace OfficeHVAC.Models
{
    public abstract class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void RemoveItem(int index)
        {
            TItem item = Items[index];
            base.RemoveItem(index);
            //FIX (NotifyCollectionChangedAction.Remove, item) throws strange exception
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, index));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }
    }
}
