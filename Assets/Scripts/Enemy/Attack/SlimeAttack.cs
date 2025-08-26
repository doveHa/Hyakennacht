using System;
using UnityEngine;

namespace Enemy.Attack
{
    public class SlimeAttack : IAttack
    {
        private Quaternion _currentState;
        public override void Attack(Vector3 targetPosition)
        {
            _currentState = transform.rotation;
            Vector3 direction = (targetPosition - transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,angle);
            transform.GetChild(1).rotation = Quaternion.Euler(0,0,(360-transform.rotation.z));
        }

        public override void Exit()
        {
            transform.rotation = _currentState;
            transform.GetChild(1).rotation = Quaternion.Euler(0,0,0);
        }
    }
}