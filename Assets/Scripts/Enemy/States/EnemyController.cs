using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public Animator Animator { get; private set; }
        private IEnemyState _currentState;
        public EnemySpawner Spawner { get; set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public Transform Target { get; private set; }

        private bool _isLeftSight;

        void Awake()
        {
            Animator = GetComponent<Animator>();
            Rigidbody = GetComponent<Rigidbody2D>();
            _isLeftSight = false;
        }

        async void Start()
        {
            EnemyStats stat = GetComponent<EnemyStats>();
            await stat.SetStat();
            Debug.Log("[" + name + "] SetRandomMoveState");
            ChangeState(new RandomMoveState(this));
        }

        void Update()
        {
            _currentState?.Update();
        }

        public void ChangeState(IEnemyState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        public void SetTarget(Transform target)
        {
            Target = target;
            ChangeState(new ChasePlayerState(this));
        }

        public void ClearTarget()
        {
            Target = null;
            ChangeState(new RandomMoveState(this));
        }

        public void Flip(bool isRightDestination)
        {
            if ((_isLeftSight && isRightDestination) || (!_isLeftSight && !isRightDestination))
            {
                _isLeftSight = !_isLeftSight;
                transform.GetChild(1).localRotation = Quaternion.Euler(0f, _isLeftSight ? 180f : 0f, 0f);
            }
        }
    }
}