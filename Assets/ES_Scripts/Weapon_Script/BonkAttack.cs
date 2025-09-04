using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonkAttack : MonoBehaviour, IWeaponBehavior
{
    public float attackRange = 1.5f;

    private WeaponData data;
    private Transform firePoint;
    private Animator animator;

    private bool isLeft;

    private Transform tailFirePoint; 
    private Vector3 originalTailPos;

    public void Initialize(WeaponData data, Transform firePoint)
    {
        this.data = data;
        this.firePoint = firePoint;

        if (data.weaponName == "����") 
        { 
            tailFirePoint = GameObject.FindWithTag("Player")?.GetComponentInChildren<WeaponHandler>()?.weaponVisualHolder; 
            if (tailFirePoint != null) 
                originalTailPos = tailFirePoint.localPosition; 
        }

        animator = FindAnimatorInWeaponVisual(firePoint?.parent?.Find("WeaponFacingProxy"));
    }

    public void Attack()
    {
        Debug.Log("Bonk Attack!");

        float range = attackRange;

        if (animator == null)
            animator = FindAnimatorInWeaponVisual(firePoint?.parent?.Find("WeaponFacingProxy"));

        if (data.weaponName == "����������")
        {
            range *= 2f; 
            if (animator != null)
            {
                animator.SetTrigger("GashaAttack"); 
            }
        }

        if (data.weaponName == "ä��")
        {
            range *= 2f;
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }

        if (data.weaponName == "���� ����")
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }

        if (data.weaponName == "����") 
        { 
            range *= 2f; 
            if (animator != null)
                animator.SetTrigger("TaliAttack"); 
            if (tailFirePoint != null) 
                StartCoroutine(MoveTailFirePoint()); 
        }

        Transform attackEffect = FindDeepChild(transform, "����");
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
                EnemyStats enemy = hit.GetComponent<EnemyStats>();
                if (enemy != null) 
                {
                    hit.GetComponent<EnemyStats>()?.TakeDamage(data.baseDamage);
                    Debug.Log($"�� ��Ʈ: {hit.name}");

                    if (data.weaponName == "���� ����")
                    {
                        if (!enemy.GetComponent<Dot>())
                            enemy.gameObject.AddComponent<Dot>().Initialize(3, 1f, "Electric");

                        /*
                        // ��ƼŬ ����Ʈ ����
                        GameObject fx = Resources.Load<GameObject>("Effects/���");
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

    private Animator FindAnimatorInWeaponVisual(Transform visualHolder)
    {
        if (visualHolder == null) return null;
        return visualHolder.GetComponentInChildren<Animator>();
    }
}



