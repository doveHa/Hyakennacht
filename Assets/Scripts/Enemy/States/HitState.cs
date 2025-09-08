using System;
using System.Collections;
using Enemy.Attack;
using UnityEngine;

namespace Enemy
{
    public class HitState : IEnemyState
    {
        private EnemyController _controller;
        private string _preState;

        private float _startTime;
        private float _waitTime = 1f;
        public HitState(EnemyController controller, IEnemyState state)
        {
            _controller = controller;
            _preState = state.GetType().Name;
        }

        public void Enter()
        {
            _controller.IsChangeState = false;
            _controller.Rigidbody.linearVelocity = Vector3.zero;
            _controller.Animator.SetTrigger("Hit");
            _startTime = Time.time;
        }

        public void Update()
        {
            if (_startTime + _waitTime < Time.time)
            {
                _controller.IsChangeState = true;
                switch (_preState)
                {
                    case "AttackState":
                        _controller.ChangeState(new AttackState(_controller));
                        break;
                    case "RandomMoveState":
                        _controller.ChangeState(new RandomMoveState(_controller));
                        break;
                    case "ChasePlayerState":
                        _controller.ChangeState(new ChasePlayerState(_controller));
                        break;
                    default:
                        _controller.ChangeState(new RandomMoveState(_controller));
                        break;
                }
            }
        }

        public void Exit()
        {
        }
    }
}