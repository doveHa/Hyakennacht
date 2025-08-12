/*
    using Enemy;
    using UnityEngine;

    public class RandomMove : MonoBehaviour
    {
        private EnemySpawner _spawner;
        private bool _isMoving;
        private Vector3 _destination;
        public float speed = 0.05f;

        void Start()
        {
            _isMoving = false;
        }

        void Update()
        {
            if (!_isMoving)
            {
                Debug.Log("new Destination");
                _destination = _spawner.GetRandomPosition();
                _isMoving = true;
            }
            
            else
            {
                Vector3 direction = (_destination - transform.position).normalized;
                GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
            }

            if (Vector3.Distance(_destination, transform.position) < 1)
            {
                _isMoving = false;
            }
        }

        public void SetSpawner(EnemySpawner spawner)
        {
            _spawner = spawner;
        }
    }*/