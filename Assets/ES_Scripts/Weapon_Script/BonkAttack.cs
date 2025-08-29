using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonkAttack : MonoBehaviour, IWeaponBehavior
{
    public float attackRange = 1.5f;

    private WeaponData data;
    private Transform firePoint;
    
    private Transform tailFirePoint; 
    private Vector3 originalTailPos;

    public void Initialize(WeaponData data, Transform firePoint)
    {
        this.data = data;
        this.firePoint = firePoint;

        if (data.weaponName == "²¿¸®") 
        { 
            tailFirePoint = GameObject.FindWithTag("Player")?.GetComponent<WeaponHandler>()?.weaponVisualHolder; 
            if (tailFirePoint != null) 
                originalTailPos = tailFirePoint.localPosition; 
        }
    }

    public void Attack()
    {
        Debug.Log("Bonk Attack!");

        float range = attackRange;

        Animator anim = GetComponentInChildren<Animator>();

        if (data.weaponName == "°¡»þµµÄí·Î")
        {
            range *= 2f; 
            if (anim != null)
            {
                anim.SetTrigger("GashaAttack"); 
            }
        }

        if (data.weaponName == "Ã¤Âï")
        {
            range *= 2f;
            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }
        }

        if (data.weaponName == "¹ø°³ ¹ßÅé")
        {
            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }
        }

        if (data.weaponName == "²¿¸®") 
        { 
            range *= 2f; 
            if (anim != null) 
                anim.SetTrigger("TaliAttack"); 
            if (tailFirePoint != null) 
                StartCoroutine(MoveTailFirePoint()); 
        }

        Transform attackEffect = FindDeepChild(transform, "°ø°Ý");
        if (attackEffect != null)
        {
            attackEffect.gameObject.SetActive(true);
            StartCoroutine(DisableAttackEffect(attackEffect));
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(firePoint.position, attackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null) 
                {
                    hit.GetComponent<Enemy>()?.TakeDamage(data.baseDamage);
                    Debug.Log($"Àû È÷Æ®: {hit.name}");

                    if (data.weaponName == "¹ø°³ ¹ßÅé")
                    {
                        if (!enemy.GetComponent<Dot>())
                            enemy.gameObject.AddComponent<Dot>().Initialize(3, 1f, "Electric");

                        /*
                        // ÆÄÆ¼Å¬ ÀÌÆåÆ® »ý¼º
                        GameObject fx = Resources.Load<GameObject>("Effects/Ãæ°Ý");
                        if (fx != null)
                            GameObject.Instantiate(fx, enemy.transform.position, Quaternion.identity);
                        */
                    }
                }
                
            }
        }
    }

    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    private IEnumerator DisableAttackEffect(Transform effect)
    {
        yield return new WaitForSeconds(0.3f); 
        if (effect != null)
            effect.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firePoint.position, attackRange);
    }

    private IEnumerator MoveTailFirePoint() 
    {
        float dirX = Mathf.Sign(transform.localScale.x);
        float dirY = transform.localScale.y;
        tailFirePoint.position = transform.position + new Vector3(2.5f * dirX, dirY * 0.5f, 0);
        yield return new WaitForSeconds(0.32f); 
        tailFirePoint.localPosition = originalTailPos; 
    }
}



