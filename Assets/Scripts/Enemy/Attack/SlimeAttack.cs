using UnityEngine;

namespace Enemy.Attack
{
    public class SlimeAttack : IAttack
    {
        private Quaternion _currentState;
        public override void Attack(Vector3 targetPosition)
        {
            _currentState = transform.rotation;
            Vector3 direction = targetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,angle);
        }

        public override void Exit()
        {
            transform.rotation = _currentState;
        }
    }
}