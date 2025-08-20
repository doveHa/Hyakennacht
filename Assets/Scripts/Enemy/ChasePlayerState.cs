using UnityEngine;

namespace Enemy
{
    public class ChasePlayerState : IEnemyState
    {
        private EnemyController _controller;

        public ChasePlayerState(EnemyController controller)
        {
            _controller = controller;
        }
        
        public void Enter()
        {
            
        }

        public void Update()
        {
            if (_controller.Target != null)
            {
                Vector3 direction = (_controller.Target.position - _controller.transform.position).normalized;
                _controller.Rigidbody.linearVelocity = direction * Constant.Enemy.MOVE_SPEED;
            }
        }

        public void Exit()
        {
            _controller.Rigidbody.linearVelocity = Vector3.zero;
        }
    }
}