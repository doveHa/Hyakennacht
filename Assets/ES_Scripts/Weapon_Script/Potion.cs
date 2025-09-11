using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public float destroyTime = 0.5f;
    public PotionEffectType potionType;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StopAllCoroutines();
            AEnemyStats aEnemy = other.GetComponent<AEnemyStats>();
            if (aEnemy != null)
            {
                ApplyEffect(aEnemy);

                Bomb(aEnemy.transform);
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
            case PotionEffectType.Heal: return "Effects/회복";
            case PotionEffectType.Poison: return "Effects/독";
            case PotionEffectType.Fire: return "Effects/불";
            case PotionEffectType.Hit: return "Effects/기본 포션";
            //case PotionType.Death: return "Effects/DeathEffect";
            default: return "";
        }
    }

    private IEnumerator NoHit()
    {
        yield return new WaitForSeconds(0.65f);
        GetComponent<SpriteRenderer>().enabled = false;
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;


        Bomb(transform);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void Bomb(Transform applyTransform)
    {
        string effectPath = GetEffectPath(potionType);
        GameObject effectPrefab = Resources.Load<GameObject>(effectPath);
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, applyTransform.position, Quaternion.identity, applyTransform);
        }
    }

    void OnEnable()
    {
        StartCoroutine(NoHit());
    }
}