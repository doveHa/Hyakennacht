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
        private List<GameObject> _prefabs;

        protected override void Awake()
        {
            base.Awake();
        }

        async void Start()
        {
            await LoadEnemyStat();
            await LoadPrefabs();
        }

        public EnemyStat GetStatByName(string enemyName)
        {
            return _enemyStats.Find(e => enemyName.Equals(e.Name));
        }

        public GameObject GetPrefabByName(string prefabName)
        {
            return _prefabs.Find(e => prefabName.Equals(e.name));
        }

        
        private async Task LoadEnemyStat()
        {
            TextAsset textAsset = await LoadAsset<TextAsset>("Assets/TextAsset/EnemyStats.json");
            string json = textAsset.text;
            _enemyStats = JsonSerializer.Deserialize<List<EnemyStat>>(json);
        }

        private async Task LoadPrefabs()
        {
            _prefabs = new List<GameObject>();
            foreach (string path in Constant.PREFAB_PATHS)
            {
                _prefabs.Add(await LoadAsset<GameObject>(path));
            }
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