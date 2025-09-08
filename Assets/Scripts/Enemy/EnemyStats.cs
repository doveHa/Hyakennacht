using UnityEngine;

namespace Enemy
{
    public class EnemyStats : AEnemyStats
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public override void TakeDamage(float dmg)
        {
            Controller.ChangeState(new HitState(Controller, Controller.CurrentState));
            CurrentHp -= dmg;
            Debug.Log(CurrentHp);
            if (CurrentHp <= 0)
            {
                Die();
            }
        }

        public override void Die()
        {
            Controller.Animator.SetTrigger("Death");
            GetComponentInParent<EnemySpawner>().KillCount++;
            Destroy(GetComponentInChildren<PlayerRecognize>());
            Destroy(GetComponent<EnemyController>());
            foreach (Collider2D cd in GetComponentsInChildren<Collider2D>())
            {
                Destroy(cd);
            }

            Destroy(GetComponent<AEnemyStats>());
        }
    }
}