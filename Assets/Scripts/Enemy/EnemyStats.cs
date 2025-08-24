using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EnemyStats : MonoBehaviour
{
    public enum EnemyName
    {
        Ghost,
        Golem,
        Kappa,
        Slime,
        Straw,
        Wisp
    }

    public EnemyName enemyName;
    private float _health;
    public float Speed { get; private set; }

    void Awake()
    {
        /* Resource, StreamReader, Addressables 비교
        using (StreamReader reader = new StreamReader(new FileStream(Constant.Path.ENEMYJSON, FileMode.Open)))
        {
            string jsonString = reader.ReadToEnd();
            Debug.Log(jsonString);
            SetStat(JsonSerializer.Deserialize<List<EnemyStat>>(jsonString));
        }*/
    }

    public async Task SetStat()
    {
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>("TextAsset/EnemyStats");
        await handle.Task;
        OnJsonLoaded(handle);
    }

    private void OnJsonLoaded(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            string json = handle.Result.text;

            List<EnemyStat> set = JsonSerializer.Deserialize<List<EnemyStat>>(json);
            EnemyStat stat = set.Find(e => e.Name == enemyName.ToString());
            _health = stat.Health;
            Speed = stat.Speed;
        }

        Addressables.Release(handle);
    }

    /*
    private void SetStat(List<EnemyStat> set)
    {
        foreach (EnemyStat enemyStat in set)
        {
            if (enemyStat.Name.Equals(enemyName.ToString()))
            {
                _health = enemyStat.Health;
                Speed = enemyStat.Speed;
                Debug.Log(enemyName.ToString());
                return;
            }
        }
    }*/

    void Start()
    {
    }

    void Update()
    {
    }

    void Hurt(float damage)
    {
        if ((_health - damage) <= 0)
        {
            Death();
        }
        else
        {
            _health -= damage;
        }
    }

    private void Death()
    {
    }

    private class EnemyStat
    {
        public string Name { get; set; }
        public float Speed { get; set; }
        public float Health { get; set; }
    }
}