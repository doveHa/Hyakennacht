using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class AttackState : IEnemyState
    {
        private EnemyController _controller;

        public AttackState(EnemyController controller)
        {
            _controller = controller;
        }

        public void Enter()
        {
            _controller.Animator.SetTrigger("Attack");
        }

        public void Update()
        {
        }

        public void Exit()
        {
        }

        public void Attack()
        {
        }
    }
}