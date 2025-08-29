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
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                ApplyEffect(enemy);
                string effectPath = GetEffectPath(potionType);
                GameObject effectPrefab = Resources.Load<GameObject>(effectPath);
                if (effectPrefab != null)
                {
                    Instantiate(effectPrefab, enemy.transform.position, Quaternion.identity, enemy.transform);
                }
            }

            Destroy(gameObject); 
        }
    }

    private void ApplyEffect(Enemy enemy)
    {
        switch (potionType)
        {
            case PotionEffectType.Heal:
                enemy.Heal(3); 
                break;

            case PotionEffectType.Poison:
                if (!enemy.GetComponent<Dot>())
                    enemy.gameObject.AddComponent<Dot>().Initialize(5, 1f, "Poison");
                break;

            case PotionEffectType.Fire:
                if (!enemy.GetComponent<Dot>())
                    enemy.gameObject.AddComponent<Dot>().Initialize(7, 1f, "Fire");
                break;

            case PotionEffectType.Hit:
                enemy.TakeDamage(10);
                break;

            case PotionEffectType.Death:
                enemy.Die(); 
                break;
        }
    }

    private string GetEffectPath(PotionEffectType type)
    {
        switch (type)
        {
            case PotionEffectType.Heal: return "Effects/회복";
            case PotionEffectType.Poison: return "Effects/독";
            case PotionEffectType.Fire: return "Effects/불";
            case PotionEffectType.Hit: return "Effects/기본 포션";
            //case PotionType.Death: return "Effects/DeathEffect";
            default: return "";
        }
    }
}
