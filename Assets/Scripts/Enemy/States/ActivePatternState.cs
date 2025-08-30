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


        public ActivePatternState(EnemyController controller, Action action)
        {
            _controller = controller;
            _action = action;
            _duration = 3f;
        }

        public void Enter()
        {
            _controller.Rigidbody.linearVelocity = Vector3.zero;
            _controller.Animator.SetTrigger("Attack");
            _timer = 0f;
        }

        public void Update()
        {
            // 패턴 실행
            _action?.Invoke();

            // 시간 체크 후 종료
            _timer += Time.deltaTime;
            if (_timer >= _duration)
            {
                // 다음 상태로 전환
                _controller.ChangeState(new RandomMoveState(_controller));
            }
        }

        public void Exit()
        {
        }
    }
}