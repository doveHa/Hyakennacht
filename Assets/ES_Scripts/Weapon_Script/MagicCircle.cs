using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircle : MonoBehaviour
{
    private float damage;
    private float damageAccumulator = 0f;

    public void SetDamage(int dmg) => damage = dmg;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy_ES enemy = other.GetComponent<Enemy_ES>();
            if (enemy != null)
            {
                damageAccumulator += damage * Time.deltaTime;

                if (damageAccumulator >= 1f)
                {
                    int intDamage = Mathf.FloorToInt(damageAccumulator);
                    enemy.TakeDamage(intDamage);
                    damageAccumulator -= intDamage;
                }
            }
        }
    }
}
