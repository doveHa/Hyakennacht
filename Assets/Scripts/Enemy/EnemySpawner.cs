using System;
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

        private void SpawnEnemies()
        {
            Random rnd = new Random((uint)System.DateTime.Now.Millisecond);
            int spawnEnemies = rnd.NextInt(Constant.SpawnEnemy.MIN_ENEMIES, Constant.SpawnEnemy.MAX_ENEMIES);
            _objects = new GameObject[spawnEnemies];
            for (int i = 0; i < spawnEnemies; i++)
            {
                int rndEnemy = rnd.NextInt(enemies.Length);
                _objects[i] = SpawnEnemy(rnd, enemies[rndEnemy]);
            }
        }

        private GameObject SpawnEnemy(Random rnd, GameObject enemy)
        {
            Vector3 local = Vector3.zero;
            do
            {
                int randomX = rnd.NextInt((int)_bounds.min.x, (int)_bounds.max.x);
                int randomY = rnd.NextInt((int)_bounds.min.y, (int)_bounds.max.y);
                Vector3Int randomPoint = new Vector3Int(randomX, randomY, 0);
                local = stage.CellToLocal(randomPoint);
            } while (CheckDuplication(enemy, local));

            return Instantiate(enemy, local, Quaternion.identity);
        }

        private bool CheckDuplication(GameObject enemy, Vector3 position)
        {
            if (Physics2D.OverlapCircleAll(position, 0.5f).Length == 0)
            {
                return false;
            }

            return true;
        }
    }
}