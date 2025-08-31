using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        private Tilemap _stage;
        private static List<GameObject> _enemyObjects;
        private Bounds _bounds;

        private GameObject[] _objects;

        public bool IsSpawn;
        private bool _isStageStart = false;

        public GameObject _wall;

        public static async Task CreateEnemies()
        {
            _enemyObjects = new List<GameObject>();

            List<string> keys = new List<string>();
            string path = "Assets/Enemy/Prefab/";

            if (StageManager.GetMapScene().Equals("YokaiMap"))
            {
                keys.Add(path + "Ghost.prefab");
                keys.Add(path + "Slime.prefab");
                keys.Add(path + "Golem.prefab");
            }
            else
            {
                keys.Add(path + "Will-o-Wisp.prefab");
                keys.Add(path + "Straw.prefab");
                keys.Add(path + "Kappa.prefab");
            }

            foreach (string key in keys)
            {
                _enemyObjects.Add(await AddressableManager.Manager.LoadAsset<GameObject>(key));
            }
        }

        void Awake()
        {
            _stage = GetComponent<Tilemap>();
            //IsSpawn = false;
        }

        void Start()
        {
            _stage.CompressBounds();
            _bounds = _stage.localBounds;
        }

        async void Update()
        {
            Vector3Int playerPoint = _stage.WorldToCell(GameManager.Manager.Player.transform.position);
            if (!IsSpawn && _stage.HasTile(playerPoint))
            {
                IsSpawn = true;
                await StartStage();
                await SpawnEnemies();
                _isStageStart = true;
            }

            if (_isStageStart && _wall.transform.childCount <= 0)
            {
                EndStage();
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

        private async Task SpawnEnemies()
        {
            Random rnd = new Random();

            int spawnEnemies = rnd.Next(Constant.SpawnEnemy.MIN_ENEMIES, Constant.SpawnEnemy.MAX_ENEMIES);
            _objects = new GameObject[spawnEnemies];

            HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>(); // ✅ 사용된 셀 기록

            for (int i = 0; i < spawnEnemies; i++)
            {
                int rndEnemy = rnd.Next(_enemyObjects.Count);
                Vector3 spawnPos = GetUniqueRandomPosition(_stage, usedPositions);
                _objects[i] = await SpawnEnemy(_enemyObjects[rndEnemy], spawnPos);
                _objects[i].transform.parent = _wall.transform;
            }
        }

        private Vector3 GetUniqueRandomPosition(Tilemap tilemap, HashSet<Vector3Int> usedPositions)
        {
            Random rnd = new Random();
            Bounds bounds = tilemap.localBounds;

            Vector3Int randomPoint;

            do
            {
                int randomX = rnd.Next((int)bounds.min.x + 1, (int)bounds.max.x);
                int randomY = rnd.Next((int)bounds.min.y + 1, (int)bounds.max.y);
                randomPoint = new Vector3Int(randomX, randomY, 0);
            } while (usedPositions.Contains(randomPoint) || !tilemap.HasTile(randomPoint));

            usedPositions.Add(randomPoint); // ✅ 사용된 위치 기록
            return tilemap.CellToLocal(randomPoint);
        }

        private async Task<GameObject> SpawnEnemy(GameObject enemy, Vector3 position)
        {
            GameObject mob = Instantiate(enemy, position, Quaternion.identity);
            mob.GetComponent<EnemyController>().Stage = _stage;
            await mob.GetComponent<EnemyStats>().SetStat();
            return mob;
        }

        public async Task StartStage()
        {
            _wall = new GameObject();
            _wall.transform.parent = transform;
            Tilemap tilemap = _wall.transform.AddComponent<Tilemap>();
            _wall.transform.AddComponent<TilemapRenderer>();
            _wall.transform.AddComponent<TilemapCollider2D>();
            TileBase tileBase = await AddressableManager.Manager.LoadAsset<TileBase>("Assets/Tile/Wall.asset");
            MapManager.Instance.GenerateWalls(GetComponent<Tilemap>(), tilemap, tileBase);
        }

        public void EndStage()
        {
            Destroy(transform.GetChild(0).gameObject);
            Destroy(this);
        }
    }
}