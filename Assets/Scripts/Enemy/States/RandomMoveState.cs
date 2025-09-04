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
            Debug.Log(direction);
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
            Bounds bounds = _controller.Stage.localBounds;
            
            int randomX = rnd.Next((int)bounds.min.x + 1, (int)bounds.max.x);
            int randomY = rnd.Next((int)bounds.min.y + 1, (int)bounds.max.y);
            Vector3Int randomPoint = new Vector3Int(randomX, randomY, 0);
            Debug.Log(randomPoint);
            return _controller.Stage.CellToLocal(randomPoint);
        }
    }
}