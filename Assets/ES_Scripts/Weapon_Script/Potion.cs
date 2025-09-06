using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public float destroyTime = 2f;
    public PotionEffectType potionType;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            AEnemyStats aEnemy = other.GetComponent<AEnemyStats>();
            if (aEnemy != null)
            {
                ApplyEffect(aEnemy);
                string effectPath = GetEffectPath(potionType);
                GameObject effectPrefab = Resources.Load<GameObject>(effectPath);
                if (effectPrefab != null)
                {
                    Instantiate(effectPrefab, aEnemy.transform.position, Quaternion.identity, aEnemy.transform);
                }
            }

            Destroy(gameObject); 
        }
    }

    private void ApplyEffect(AEnemyStats aEnemy)
    {
        switch (potionType)
        {
            case PotionEffectType.Heal:
                aEnemy.Heal(3); 
                break;

            case PotionEffectType.Poison:
                if (!aEnemy.GetComponent<Dot>())
                    aEnemy.gameObject.AddComponent<Dot>().Initialize(5, 1f, "Poison");
                break;

            case PotionEffectType.Fire:
                if (!aEnemy.GetComponent<Dot>())
                    aEnemy.gameObject.AddComponent<Dot>().Initialize(7, 1f, "Fire");
                break;

            case PotionEffectType.Hit:
                aEnemy.TakeDamage(10);
                break;

            case PotionEffectType.Death:
                aEnemy.Die(); 
                break;
        }
    }

    private string GetEffectPath(PotionEffectType type)
    {
        switch (type)
        {
            case PotionEffectType.Heal: return "Effects/ȸ��";
            case PotionEffectType.Poison: return "Effects/��";
            case PotionEffectType.Fire: return "Effects/��";
            case PotionEffectType.Hit: return "Effects/�⺻ ����";
            //case PotionType.Death: return "Effects/DeathEffect";
            default: return "";
        }
    }
}
