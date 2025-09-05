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
        private List<GameObject> _enemyObjects;
        private Bounds _bounds;

        private GameObject[] _objects;

        public bool IsSpawn;
        private bool _isStageStart = false;

        private int _enemyCount;
        public int KillCount { get; set; } = -1;
        private GameObject _wall,_enemies;

        void Awake()
        {
            _stage = GetComponent<Tilemap>();
        }

        void Start()
        {
            _stage.CompressBounds();
            _bounds = _stage.localBounds;

            _enemyObjects = new List<GameObject>();

            foreach (GameObject enemy in EnemyPrefabs.Instance.EnemyPrefab)
            {
                _enemyObjects.Add(enemy);
            }
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
                KillCount = 0;
            }

            if (KillCount == _enemyCount)
            {
                EndStage();
            }
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
                _objects[i].transform.parent = _enemies.transform;
                _enemyCount++;
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
            _wall.name = "Wall";
            _enemies = new GameObject();
            _enemies.transform.parent = transform;
            _enemies.name = "Enemies";
            
            Tilemap tilemap = _wall.transform.AddComponent<Tilemap>();
            _wall.transform.AddComponent<TilemapRenderer>().sortingOrder = 1;
            _wall.transform.AddComponent<TilemapCollider2D>();
            TileBase tileBase = MapManager.Instance.wallTileHorizontal;
            MapManager.Instance.GenerateWalls(GetComponent<Tilemap>(), tilemap, tileBase);
        }

        public void EndStage()
        {
            Destroy(transform.Find("Wall").gameObject);
            Destroy(this);
        }
    }
}