using System;

using UnityEngine;

namespace Utils
{
    [Serializable]
    public class KeyValueS<TKey, TValue>
    {
        [SerializeField]
        private TKey key;

        public TKey Key => key;

        [SerializeField]
        private TValue value;

        public TValue Value => value;
    }
}