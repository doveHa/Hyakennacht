using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Manager;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = Unity.Mathematics.Random;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        private Tilemap _stage;
        private List<GameObject> _enemyObjects;
        private Bounds _bounds;

        private GameObject[] _objects;
        private Random _rnd;

        private bool _isSpawn;

        void Awake()
        {
            _rnd = new Random((uint)DateTime.Now.Millisecond);
            _stage = GetComponent<Tilemap>();
            _enemyObjects = new List<GameObject>();
            _isSpawn = false;
        }

        async void Start()
        {
            List<string> keys = new List<string>();
            string path = "Assets/Enemy/Prefab/";
            keys.Add(path + "Ghost.prefab");
            keys.Add(path + "Will-o-Wisp.prefab");
            keys.Add(path + "Straw.prefab");
            keys.Add(path + "Kappa.prefab");

            foreach (string key in keys)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
                await handle.Task;
                _enemyObjects.Add(handle.Result);
            }

            _stage.CompressBounds();
            _bounds = _stage.localBounds;
            Debug.Log("max" + _bounds.max);
            Debug.Log("min" + _bounds.min);
            Debug.Log("size" + _bounds.size);
        }

        void Update()
        {
            Vector3Int playerPoint = _stage.WorldToCell(GameManager.Manager.Player.transform.position);
            if (!_isSpawn && _stage.HasTile(playerPoint))
            {
                _isSpawn = true;
                SpawnEnemies();
            }
            /*
            if (Input.GetKeyDown(KeyCode.R))
            {
                foreach (GameObject obj in _objects)
                {
                    Destroy(obj);
                }

                SpawnEnemies();
            }*/
        }

        public Vector3 GetRandomPosition()
        {
            Vector3 randomPosition;
            
            int randomX = _rnd.NextInt((int)_bounds.min.x, (int)_bounds.max.x);
            int randomY = _rnd.NextInt((int)_bounds.min.y, (int)_bounds.max.y);
            Vector3Int randomPoint = new Vector3Int(randomX, randomY, 0);
            randomPosition = _stage.CellToLocal(randomPoint);
            /*
            do
            {
                int randomX = _rnd.NextInt((int)_bounds.min.x, (int)_bounds.max.x);
                int randomY = _rnd.NextInt((int)_bounds.min.y, (int)_bounds.max.y);
                Vector3Int randomPoint = new Vector3Int(randomX, randomY, 0);
                randomPosition = _stage.CellToLocal(randomPoint);
            } while (CheckDuplication(randomPosition));
*/
            return randomPosition;
        }


        private async Task SpawnEnemies()
        {
            int spawnEnemies = _rnd.NextInt(Constant.SpawnEnemy.MIN_ENEMIES, Constant.SpawnEnemy.MAX_ENEMIES);
            _objects = new GameObject[spawnEnemies];
            for (int i = 0; i < spawnEnemies; i++)
            {
                int rndEnemy = _rnd.NextInt(_enemyObjects.Count);
                _objects[i] = await SpawnEnemy(_enemyObjects[rndEnemy]);
            }
        }

        private async Task<GameObject> SpawnEnemy(GameObject enemy)
        {
            Vector3 local = GetRandomPosition();
            GameObject mob = Instantiate(enemy, local, Quaternion.identity);
            mob.GetComponent<EnemyController>().Spawner = this;
            await mob.GetComponent<EnemyStats>().SetStat();
            return mob;
        }

        private bool CheckDuplication(Vector3 position)
        {
            if (Physics2D.OverlapCircleAll(position, Constant.SpawnEnemy.SPAWN_DUPLICATION_DISTANCE).Length == 0)
            {
                return false;
            }

            return true;
        }
    }
}