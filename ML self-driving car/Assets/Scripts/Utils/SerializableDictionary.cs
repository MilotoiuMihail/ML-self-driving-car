using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [System.Serializable]
    private class KeyValue
    {
        [SerializeField] private TKey key = default(TKey);
        [SerializeField] private TValue value = default(TValue);

        public TKey Key { get { return key; } }
        public TValue Value { get { return value; } }
        public KeyValue()
        {

        }
        public KeyValue(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }
    [SerializeField]
    private List<KeyValue> data = new List<KeyValue>();
    private KeyValue defaultItem;
    private bool addedFirstItem;
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        data.Clear();
        foreach (var pair in this)
        {
            if (!addedFirstItem)
            {
                defaultItem = new KeyValue(pair.Key, pair.Value);
                addedFirstItem = true;
            }
            data.Add(new KeyValue(pair.Key, pair.Value));
        }
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        Clear();
        foreach (var item in data)
        {
            if (this.ContainsKey(item.Key))
            {
                this[defaultItem.Key] = defaultItem.Value;
            }
            else
            {
                this[item.Key] = item.Value;
            }
        }
    }
}