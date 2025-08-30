using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
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

        public bool IsSpawn;

        void Awake()
        {
            _stage = GetComponent<Tilemap>();
            _enemyObjects = new List<GameObject>();
            //IsSpawn = false;
        }

        async void Start()
        {
            List<string> keys = new List<string>();
            string path = "Assets/Enemy/Prefab/";
            //keys.Add(path + "Ghost.prefab");
            keys.Add(path + "Will-o-Wisp.prefab");
            keys.Add(path + "Straw.prefab");
            keys.Add(path + "Kappa.prefab");
            //keys.Add(path + "Slime.prefab");
            //keys.Add(path + "Golem.prefab");
            foreach (string key in keys)
            {
                _enemyObjects.Add(await AddressableManager.Manager.LoadAsset<GameObject>(key));
            }

            _stage.CompressBounds();
            _bounds = _stage.localBounds;
        }

        void Update()
        {
            Vector3Int playerPoint = _stage.WorldToCell(GameManager.Manager.Player.transform.position);
            if (!IsSpawn && _stage.HasTile(playerPoint))
            {
                IsSpawn = true;
                StartStage();
                SpawnEnemies();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                EndStage();
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

        public static Vector3 GetRandomPosition(Tilemap tilemap)
        {
            Random rnd = new Random((uint)DateTime.Now.Millisecond);
            Bounds bounds = tilemap.localBounds;
            Vector3 randomPosition;

            int randomX = rnd.NextInt((int)bounds.min.x, (int)bounds.max.x);
            int randomY = rnd.NextInt((int)bounds.min.y, (int)bounds.max.y);
            Vector3Int randomPoint = new Vector3Int(randomX, randomY, 0);
            randomPosition = tilemap.CellToLocal(randomPoint);

            return randomPosition;
        }


        private async Task SpawnEnemies()
        {
            Random rnd = new Random((uint)DateTime.Now.Millisecond);

            int spawnEnemies = rnd.NextInt(Constant.SpawnEnemy.MIN_ENEMIES, Constant.SpawnEnemy.MAX_ENEMIES);
            _objects = new GameObject[spawnEnemies];

            HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>(); // ✅ 사용된 셀 기록

            for (int i = 0; i < spawnEnemies; i++)
            {
                int rndEnemy = rnd.NextInt(_enemyObjects.Count);
                Vector3 spawnPos = GetUniqueRandomPosition(_stage, usedPositions);
                _objects[i] = await SpawnEnemy(_enemyObjects[rndEnemy], spawnPos);
            }
        }

        private Vector3 GetUniqueRandomPosition(Tilemap tilemap, HashSet<Vector3Int> usedPositions)
        {
            Random rnd = new Random((uint)DateTime.Now.Millisecond);
            Bounds bounds = tilemap.localBounds;

            Vector3Int randomPoint;

            // 중복되지 않는 타일 좌표 뽑기
            do
            {
                int randomX = rnd.NextInt((int)bounds.min.x, (int)bounds.max.x);
                int randomY = rnd.NextInt((int)bounds.min.y, (int)bounds.max.y);
                randomPoint = new Vector3Int(randomX, randomY, 0);
            } while (usedPositions.Contains(randomPoint) || !tilemap.HasTile(randomPoint));

            usedPositions.Add(randomPoint); // ✅ 사용된 위치 기록
            return tilemap.CellToLocal(randomPoint);
        }

        private async Task<GameObject> SpawnEnemy(GameObject enemy, Vector3 position)
        {
            GameObject mob = Instantiate(enemy, position, Quaternion.identity);
            mob.GetComponent<EnemyController>().stage = _stage;
            await mob.GetComponent<EnemyStats>().SetStat();
            return mob;
        }


        public async void StartStage()
        {
            GameObject child = new GameObject();
            child.transform.parent = transform;
            TileBase tileBase = await AddressableManager.Manager.LoadAsset<TileBase>("Assets/Tile/Wall.asset");
            Tilemap tilemap = child.transform.AddComponent<Tilemap>();
            child.transform.AddComponent<TilemapRenderer>().sortingOrder = 2;
            child.transform.AddComponent<TilemapCollider2D>();
            MapManager.Instance.GenerateWalls(GetComponent<Tilemap>(), tilemap, tileBase,true);
            //await SpawnEnemies();
        }

        public void EndStage()
        {
            Destroy(transform.GetChild(0).gameObject);
            Destroy(this);
        }
    }
}