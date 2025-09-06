using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Manager;
using UnityEngine;

namespace Enemy.BossStage
{
    public class BossStat : AEnemyStats
    {
        private BossHpBar _bossHpBar;
        
        protected override void Awake()
        {
            base.Awake();
        }
        
        public override void TakeDamage(float dmg)
        {
            CurrentHp -= dmg;
            _bossHpBar.TakeDamage(dmg);
            
            if (CurrentHp <= 0)
            {
                Die();
            }
        }

        public override void Die()
        {
            Controller.Animator.SetTrigger("Death");
            Destroy(GetComponent<EnemyController>());
            Destroy(GetComponent<BossPatternController>());
            Destroy(GetComponent<ABossPattern>());
            foreach (Collider2D cd in GetComponentsInChildren<Collider2D>())
            {
                Destroy(cd);
            }

            Destroy(GetComponent<AEnemyStats>());
        }

        public void SetBossHpBar(BossHpBar bossHpBar)
        {
            _bossHpBar = bossHpBar;
            _bossHpBar.SetHp(MaxHp);
        }
    }
}