using System;

using UnityEngine;

namespace Utils
{
    public abstract class MonoBehaviourMgr<T> : MonoBehaviour
    {
        public static T Instance
        {
            get;
            private set;
        }

        protected virtual T InitInstance
        {
            get
            {
                if (this is T t)
                    return t;

                throw new();
            }
        }

        protected virtual void Awake()
        {
            Instance = InitInstance;
        }
    }
}