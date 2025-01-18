using System;
using System.Collections.Generic;

namespace NTE.Utils
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public delegate void OnItemChangedEventHandler(OnItemChangedEventArgs<TKey, TValue> e);

        public event OnItemChangedEventHandler OnItemChanged;

        public new TValue this[TKey key]
        {
            get => base[key];
            set => Set(key, value);
        }

        public void Set(TKey key, TValue item)
        {
            base[key] = item;
            OnItemChanged?.Invoke(new(ItemChangeType.Set, key, item));
        }

        public new void Add(TKey key, TValue item)
        {
            base.Add(key, item);
            OnItemChanged?.Invoke(new(ItemChangeType.Add, key, item));
        }

        public new void Remove(TKey key)
        {
            TValue item = base[key];
            base.Remove(key);
            OnItemChanged?.Invoke(new(ItemChangeType.Remove, key, item));
        }

    }

    public class OnItemChangedEventArgs<TKey, TValue> : EventArgs
    {
        public ItemChangeType Type;
        public TKey ChangedIndex;
        public TValue ChangedItem;

        public OnItemChangedEventArgs(ItemChangeType type, TKey changedIndex, TValue changedItem)
        {
            Type = type;
            ChangedIndex = changedIndex;
            ChangedItem = changedItem;
        }
    }

    public enum ItemChangeType
    {
        Add,
        Remove,
        Set
    }
}