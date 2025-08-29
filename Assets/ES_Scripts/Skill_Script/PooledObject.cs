using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public string key;
    public ObjectPool pool;

    public void ReturnToPool()
    {
        pool?.Despawn(key, gameObject);
    }
}
