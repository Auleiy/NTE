using UnityEngine;

namespace NTE.Core.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Initialized)
                Destroy(gameObject);
            else
                Instance = (T)this;
        }

        public static bool Initialized
        {
            get { return Instance != null; }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
