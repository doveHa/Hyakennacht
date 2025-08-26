using System;
using System.Collections;
using UnityEngine;

namespace Enemy.Attack
{
    public class GhostAttack : IAttack
    {
        public float attackSpeed;
        public override void Attack(Vector3 targetPosition)
        {
            StartCoroutine(Charge(targetPosition));
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