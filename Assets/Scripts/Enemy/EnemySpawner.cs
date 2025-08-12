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
            int randomX = rnd.NextInt((int)_bounds.min.x, (int)_bounds.max.x);
            int randomY = rnd.NextInt((int)_bounds.min.y, (int)_bounds.max.y);

            Vector3Int randomPoint = new Vector3Int(randomX, randomY, 0);
            Vector3 local = stage.CellToLocal(randomPoint);
            return Instantiate(enemy, local, Quaternion.identity);
        }

        private bool CheckDuplication(Vector3 position)
        {
            GameObject empty = Instantiate(new GameObject(), position, Quaternion.identity);
            empty.AddComponent<Collider2D>();
            empty.AddComponent<Rigidbody2D>().gravityScale = 0;
            CheckCollider checkCollider = empty.AddComponent<CheckCollider>();
            return checkCollider.IsOverlap;
        }

        class CheckCollider : MonoBehaviour
        {
            public bool IsOverlap { get; private set; } = false;
            private void OnCollisionStay2D(Collision2D other)
            {
                IsOverlap = true;
            }
        }
    }
}