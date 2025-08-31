using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public Animator Animator { get; private set; }
        private IEnemyState _currentState;
        public Tilemap stage;
        public Rigidbody2D Rigidbody { get; private set; }
        public Transform Target { get; private set; }

        public bool IsChangeState = true;
        public bool IsLeftSight;

        void Awake()
        {
            Animator = GetComponent<Animator>();
            Rigidbody = GetComponent<Rigidbody2D>();
            IsLeftSight = false;
        }

        async void Start()
        {
            EnemyStats stat = GetComponent<EnemyStats>();
            await stat.SetStat();
            ChangeState(new RandomMoveState(this));
        }

        void Update()
        {
            _currentState?.Update();
        }

        public void ChangeState(IEnemyState newState)
        {
            if (IsChangeState)
            {
                _currentState?.Exit();
                _currentState = newState;
                _currentState?.Enter();
            }
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
            if ((IsLeftSight && isRightDestination) || (!IsLeftSight && !isRightDestination))
            {
                IsLeftSight = !IsLeftSight;
                transform.GetChild(1).localRotation = Quaternion.Euler(0f, IsLeftSight ? 180f : 0f, 0f);
            }
        }

        public void CanChangeState()
        {
            IsChangeState = true;
        }
    }
}