using System;
using System.Collections;
using Enemy.Attack;
using UnityEngine;

namespace Enemy
{
    public class AttackState : IEnemyState
    {
        private EnemyController _controller;
        private IAttack _attack;

        public AttackState(EnemyController controller)
        {
            _controller = controller;
            _attack = _controller.GetComponent<IAttack>();
        }

        public void Enter()
        {
            _attack.Attack(_controller.Target.position);
            _controller.Animator.SetTrigger("Attack");
        }

        public void Update()
        {
        }

        public void Exit()
        {
            _attack.Exit();
        }
    }
}