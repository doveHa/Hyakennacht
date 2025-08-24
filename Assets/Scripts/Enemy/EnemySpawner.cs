using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = Unity.Mathematics.Random;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public Tilemap stage;
        public GameObject[] enemies;
        private Bounds _bounds;

        private GameObject[] _objects;
        private Random _rnd;

        void Awake()
        {
            _rnd = new Random((uint)DateTime.Now.Millisecond);
        }

        void Start()
        {
            _bounds = stage.localBounds;
            SpawnEnemies();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                foreach (GameObject obj in _objects)
                {
                    Destroy(obj);
                }

                SpawnEnemies();
            }
        }

        public Vector3 GetRandomPosition()
        {
            Vector3 randomPosition;
            do
            {
                int randomX = _rnd.NextInt((int)_bounds.min.x, (int)_bounds.max.x);
                int randomY = _rnd.NextInt((int)_bounds.min.y, (int)_bounds.max.y);
                Vector3Int randomPoint = new Vector3Int(randomX, randomY, 0);
                randomPosition = stage.CellToLocal(randomPoint);
            } while (CheckDuplication(randomPosition));

            return randomPosition;
        }


        private async Task SpawnEnemies()
        {
            int spawnEnemies = _rnd.NextInt(Constant.SpawnEnemy.MIN_ENEMIES, Constant.SpawnEnemy.MAX_ENEMIES);
            _objects = new GameObject[spawnEnemies];
            for (int i = 0; i < spawnEnemies; i++)
            {
                int rndEnemy = _rnd.NextInt(enemies.Length);
                _objects[i] = await SpawnEnemy(enemies[rndEnemy]);
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