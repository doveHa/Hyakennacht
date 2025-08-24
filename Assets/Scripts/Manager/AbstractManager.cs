using UnityEngine;

namespace Manager
{
    public abstract class AbstractManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Manager { get; private set; }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            if (!GameObject.Find("Manager").TryGetComponent<T>(out T manager) && Manager == null)
            {
                GameObject.Find("Manager").AddComponent<T>();
                Destroy(gameObject);
            }
            
            Manager = GameObject.Find("Manager").GetComponent<T>();

        }
    }
}