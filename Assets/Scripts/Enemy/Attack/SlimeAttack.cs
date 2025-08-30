using System;
using System.Collections;
using UnityEngine;

namespace Enemy.Attack
{
    public class SlimeAttack : IAttack
    {
        public float attackSpeed;

        public override void Attack(Vector3 targetPosition)
        {
            StartCoroutine(Charge(targetPosition));
            /*
            _currentState = transform.rotation;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, transform.GetChild(1).eulerAngles.y, angle);
            Debug.Log(transform.eulerAngles.z);
            Debug.Log(360 - angle);
            transform.GetChild(1).localRotation = Quaternion.AngleAxis(360 - angle, Vector3.forward);

        */
        }

        private IEnumerator Charge(Vector3 targetPosition)
        {
            Vector2 direction = (targetPosition - transform.position);

            while (Vector2.Distance(targetPosition, transform.position) > 1f)
            {
                GetComponent<Rigidbody2D>().linearVelocity = direction * attackSpeed;
                yield return null;
            }

            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            GetComponent<EnemyController>().ChangeState(new ChasePlayerState(GetComponent<EnemyController>()));
        }
        
        public override void Exit()
        {
            base.Exit();
        }
    }
}