using System;
using Enemy.Attack;
using UnityEngine;

namespace Enemy
{
    public class ActivePatternState : IEnemyState
    {
        private EnemyController _controller;
        private Action _action;

        public float _duration; // 패턴 지속 시간
        private float _timer;

        private int _patternIndex;

        public ActivePatternState(EnemyController controller, Action action, int patternIndex)
        {
            _controller = controller;
            _action = action;
            _duration = 3f;
            _patternIndex = patternIndex;
        }

        public void Enter()
        {
            _controller.Rigidbody.linearVelocity = Vector3.zero;
            _controller.Animator.SetTrigger("Attack");
            _controller.Animator.SetInteger("PatternIndex", _patternIndex + 1);
            _action?.Invoke();
            _controller.IsChangeState = false;
            _timer = 0f;
        }

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration)
            {
                _controller.ChangeState(new RandomMoveState(_controller));
            }
        }

        public void Exit()
        {
        }
    }
}