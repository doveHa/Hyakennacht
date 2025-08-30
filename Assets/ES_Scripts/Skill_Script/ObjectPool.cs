using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        public string key;
        public GameObject prefab;
        public int preload = 8;
    }

    public List<Entry> entries = new List<Entry>();

    private readonly Dictionary<string, Queue<GameObject>> pools = new();
    private readonly Dictionary<string, Entry> entryMap = new();

    void Awake()
    {
        foreach (var e in entries)
        {
            if (string.IsNullOrEmpty(e.key) || e.prefab == null) continue;
            entryMap[e.key] = e;

            var q = new Queue<GameObject>();
            for (int i = 0; i < Mathf.Max(0, e.preload); i++)
            {
                var go = Create(e.key);
                go.SetActive(false);
                q.Enqueue(go);
            }
            pools[e.key] = q;
        }
    }

    GameObject Create(string key)
    {
        if (!entryMap.TryGetValue(key, out var e) || e.prefab == null) return null;
        var go = Instantiate(e.prefab, transform);

        var token = go.GetComponent<PooledObject>() ?? go.AddComponent<PooledObject>();
        token.key = key;
        token.pool = this;

        return go;
    }

    public GameObject Spawn(string key)
    {
        if (!pools.TryGetValue(key, out var q))
        {
            // 키가 처음이면 큐 만들기
            q = new Queue<GameObject>();
            pools[key] = q;
        }

        var go = q.Count > 0 ? q.Dequeue() : Create(key);
        if (go == null) return null;

        go.SetActive(true);
        return go;
    }

    public void Despawn(string key, GameObject go)
    {
        if (go == null) return;
        if (!pools.TryGetValue(key, out var q))
        {
            q = new Queue<GameObject>();
            pools[key] = q;
        }
        go.SetActive(false);
        q.Enqueue(go);
    }

    public void Despawn(GameObject go)
    {
        if (go == null) return;
        var token = go.GetComponent<PooledObject>();
        if (token != null) Despawn(token.key, go);
        else go.SetActive(false); // 토큰 없으면 최소 안전
    }
}

