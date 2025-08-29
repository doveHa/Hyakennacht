using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXRouter : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        public string key;
        public GameObject prefab;
    }

    public List<Entry> effects;

    public void PlayAt(string key, Vector2 pos)
    {
        var entry = effects.Find(e => e.key == key);
        if (entry == null) return;

        var go = Instantiate(entry.prefab, pos, Quaternion.identity);
        
        Destroy(go, 2f);
    }
}
