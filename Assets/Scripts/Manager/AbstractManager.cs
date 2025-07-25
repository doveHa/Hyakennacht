using UnityEngine;

namespace Manager
{
    public abstract class AbstractManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Manager { get; private set; }

        protected virtual void Awake()
        {
            if (Manager == null)
            {
                Manager = this as T;
            }
        }
    }
}