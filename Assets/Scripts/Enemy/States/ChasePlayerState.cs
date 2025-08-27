using System.IO;
using UnityEngine;

namespace Enemy
{
    public class ChasePlayerState : IEnemyState
    {
        private float _enemySpeed;
        private EnemyController _controller;

        public ChasePlayerState(EnemyController controller)
        {
            _controller = controller;
            _enemySpeed = _controller.transform.GetComponent<EnemyStats>().Speed;
        }
        
        public void Enter()
        {
            _controller.Animator.SetBool("IsWalk",true);
        }

        public void Update()
        {
            if (_controller.Target != null)
            {
                Vector3 direction = (_controller.Target.position - _controller.transform.position).normalized;
                _controller.Rigidbody.linearVelocity = direction * _enemySpeed;

                if (Vector2.Distance(_controller.Target.position, _controller.transform.position) < 1)
                {
                    _controller.Rigidbody.linearVelocity = Vector3.zero;
                    _controller.ChangeState(new AttackState(_controller));
                }
            }
        }

        public void Exit()
        {
            _controller.Rigidbody.linearVelocity = Vector3.zero;
            _controller.Animator.SetBool("IsWalk", false);

        }
    }
}