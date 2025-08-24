using UnityEngine;

namespace Enemy
{
    public class RandomMoveState : IEnemyState
    {
        private float _enemySpeed;
        private EnemyController _controller;
        private Vector3 _destination;

        public RandomMoveState(EnemyController controller)
        {
            Debug.Log("RandomMoveState");
            _controller = controller;
            _controller.Animator.SetBool("IsWalk", true);
            _enemySpeed = _controller.transform.GetComponent<EnemyStats>().Speed;
        }

        public void Enter()
        {
            _destination = _controller.Spawner.GetRandomPosition();
            _controller.Flip(_controller.transform.position.x < _destination.x);
        }

        public void Update()
        {
            Vector3 direction = (_destination - _controller.transform.position).normalized;
            _controller.Rigidbody.linearVelocity = direction * _enemySpeed;
            if (Vector3.Distance(_destination, _controller.transform.position) < 1f)
            {
                _controller.Rigidbody.linearVelocity = Vector3.zero;
                _controller.ChangeState(new IdleState(_controller));
            }
        }

        public void Exit()
        {
            _controller.Animator.SetBool("IsWalk", false);
        }
    }
}