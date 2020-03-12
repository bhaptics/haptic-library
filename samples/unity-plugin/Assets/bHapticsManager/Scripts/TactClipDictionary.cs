using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Bhaptics.Tact.Unity
{
    [Serializable]
    public class TactClipContainer
    {
        public string Key;
        public TactClip Value;

        public TactClipContainer()
        {
            Key = "";
            Value = null;
        }

        public TactClipContainer(string _Key, TactClip _Value)
        {
            Key = _Key;
            Value = _Value;
        }
    }

    [Serializable]
    public class TactClipDictionary
    {
        public List<TactClipContainer> Items;

        public TactClipDictionary()
        {
            Items = new List<TactClipContainer>();
        }




        public bool Add(TactClipContainer item)
        {
            if (item != null && item.Value != null)
            {
                return Add(item.Key, item.Value);
            }
            return false;
        }

        public bool Add(string _Key, TactClip _Value)
        {
            if (Items == null)
            {
                return false;
            }
            if (_Value != null)
            {
                for (int i = 0; i < Items.Count; ++i)
                {
                    if (Items[i].Key == _Key)
                    {
                        Items[i].Value = _Value;
                        return true;
                    }
                }
                Items.Add(new TactClipContainer(_Key, _Value));
                return true;
            }
            return false;
        }

        public bool Contains(string _Key)
        {
            if (Items == null)
            {
                return false;
            }
            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i].Key == _Key)
                {
                    return true;
                }
            }
            return false;
        }

        public TactClip GetValue(string _Key)
        {
            if (Items == null)
            {
                return null;
            }
            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i].Key == _Key)
                {
                    return Items[i].Value;
                }
            }
            return null;
        }
    }
}