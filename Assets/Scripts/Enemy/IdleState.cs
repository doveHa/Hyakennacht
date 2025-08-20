using UnityEngine;

namespace Enemy
{
    public class IdleState : IEnemyState
    {
        private EnemyController _controller;
        private int _currentFrame;
        private int _waitFrame = 300;

        public IdleState(EnemyController controller)
        {
            _controller = controller;
        }

        public void Enter()
        {
            _currentFrame = 0;
        }

        public void Update(){
            if (_currentFrame < _waitFrame)
            {
                _currentFrame++;
            }
            else
            {
                _controller.ChangeState(new RandomMoveState(_controller));
            }
        }

        public void Exit()
        {
        }
    }
}