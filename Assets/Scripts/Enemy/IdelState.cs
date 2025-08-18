using System.Threading;
using UnityEngine;

namespace Enemy
{
    public class IdelState : IEnemyState
    {
        private EnemyController _controller;

        public IdelState(EnemyController controller)
        {
            _controller = controller;
            _controller.Animator.SetTrigger("Idle");
        }

        public void Enter()
        {
            Thread.Sleep(2000);
            Debug.Log("Move");
            _controller.ChangeState(new RandomMoveState(_controller));
        }

        public void Update(){
        }

        public void Exit()
        {
        }
    }
}