using System;
using UnityEngine;

namespace Enemy.Attack
{
    public class GolemAttack : IAttack
    {
        public override void Attack(Vector3 targetPosition)
        {
            Transform attackTransform = transform.GetChild(2);
            Vector3 direction = (targetPosition - transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            attackTransform.rotation = Quaternion.Euler(0, 0, angle);

            SpriteRenderer sr = attackTransform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.flipY = direction.x < 0;
            }
        }

        public override void Exit()
        {            base.Exit();

        }
    }
}