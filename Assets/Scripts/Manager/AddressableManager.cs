using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Manager
{
    public class AddressableManager : AbstractManager<AddressableManager>
    {
        private List<EnemyStat> _enemyStats;

        protected override void Awake()
        {
            base.Awake();
        }

        async void Start()
        {
            await LoadEnemyStat();
        }

        public EnemyStat GetStatByName(string enemyName)
        {
            return _enemyStats.Find(e => enemyName == e.Name);
        }

        private async Task LoadEnemyStat()
        {
            TextAsset textAsset = await LoadAsset<TextAsset>("Assets/TextAsset/EnemyStats.json");
            string json = textAsset.text;
            _enemyStats = JsonSerializer.Deserialize<List<EnemyStat>>(json);
        }

        private async Task<T> LoadAsset<T>(string key)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;
            T t = handle.Result;
            Addressables.Release(handle);
            return t;
        }


        public class EnemyStat
        {
            public EnemyStat(string name, float speed, float health)
            {
                Name = name;
                Speed = speed;
                Health = health;
            }

            public string Name { get; set; }
            public float Speed { get; set; }
            public float Health { get; set; }
        }
    }
}