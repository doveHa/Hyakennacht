using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public Animator Animator { get; private set; }
        public IEnemyState CurrentState { get; private set; }

        public Tilemap Stage { get; set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public Transform Target { get; private set; }

        public bool IsChangeState { get; set; }
        public bool IsLeftSight;

        void Awake()
        {
            Animator = GetComponent<Animator>();
            Rigidbody = GetComponent<Rigidbody2D>();
            IsChangeState = true;
            IsLeftSight = false;
        }

        void Start()
        {
           ChangeState(new RandomMoveState(this));
        }

        void Update()
        {
            CurrentState?.Update();
        }

        public void ChangeState(IEnemyState newState)
        {
            if (IsChangeState)
            {
                CurrentState?.Exit();
                CurrentState = newState;
                CurrentState?.Enter();
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
/*
        public void CanChangeState()
        {
            IsChangeState = true;
        }
*/
    }
}