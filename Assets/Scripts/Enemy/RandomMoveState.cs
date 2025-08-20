using UnityEngine;

namespace Enemy
{
    public class RandomMoveState : IEnemyState
    {
        private EnemyController _controller;
        private Vector3 _destination;

        public RandomMoveState(EnemyController controller)
        {
            _controller = controller;
            _controller.Animator.SetBool("IsWalk",true);
        }

        public void Enter()
        {
            _destination = _controller.Spawner.GetRandomPosition();
        }

        public void Update()
        {
            Vector3 direction = (_destination - _controller.transform.position).normalized;
            _controller.Rigidbody.linearVelocity = direction * Constant.Enemy.MOVE_SPEED;
            if (Vector3.Distance(_destination, _controller.transform.position) < 1f)
            {
                _controller.ChangeState(new IdleState(_controller));
            }
        }

        public void Exit()
        {
            _controller.Animator.SetBool("IsWalk",false);
        }
    }
}