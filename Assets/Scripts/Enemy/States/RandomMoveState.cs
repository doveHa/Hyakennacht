using System;
using UnityEngine;
using Random = System.Random;

namespace Enemy
{
    public class RandomMoveState : IEnemyState
    {
        private float _enemySpeed;
        private EnemyController _controller;
        private Vector3 _destination;

        public RandomMoveState(EnemyController controller)
        {
            _controller = controller;
            _controller.Animator.SetBool("IsWalk", true);
            _enemySpeed = _controller.transform.GetComponent<EnemyStats>().Speed;
        }

        public void Enter()
        {
            _destination = GetRandomPosition();
            _controller.Flip(_controller.transform.position.x < _destination.x);
        }

        public void Update()
        {
            Vector3 direction = (_destination - _controller.transform.position).normalized;
            _controller.Rigidbody.linearVelocity = direction * _enemySpeed;
            if (Vector3.Distance(_destination, _controller.transform.position) < 3f)
            {
                _controller.Rigidbody.linearVelocity = Vector3.zero;
                _controller.ChangeState(new IdleState(_controller));
            }
        }

        public void Exit()
        {
            _controller.Animator.SetBool("IsWalk", false);
        }
        
        public Vector3 GetRandomPosition()
        {
            Random rnd = new Random();
            Bounds bounds = _controller.stage.localBounds;
            int minX = Mathf.Min((int)_controller.stage.localBounds.min.x, (int)_controller.stage.localBounds.max.x);
            int maxX = Mathf.Max((int)_controller.stage.localBounds.min.x, (int)_controller.stage.localBounds.max.x);
            int minY = Mathf.Min((int)_controller.stage.localBounds.min.y, (int)_controller.stage.localBounds.max.y);
            int maxY = Mathf.Max((int)_controller.stage.localBounds.min.y, (int)_controller.stage.localBounds.max.y);
            
            int randomX = rnd.Next((int)bounds.min.x, (int)bounds.max.x + 1);
            int randomY = rnd.Next((int)bounds.min.y, (int)bounds.max.y + 1);
            Vector3Int randomPoint = new Vector3Int(randomX, randomY, 0);
            Debug.Log(randomPoint);
            return _controller.stage.CellToLocal(randomPoint);
        }
    }
}