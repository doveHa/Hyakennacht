using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    public float duration = 3f; // 지속 시간
    public float tickInterval = 0.5f;
    private int damage;

    private Transform target;
    private Animator animator;

    public void SetDamage(int dmg) => damage = dmg;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (target == null && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                target = other.transform;
                transform.SetParent(target);
                transform.localPosition = Vector3.zero;

                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.isKinematic = true;
                }

                Collider2D col = GetComponent<Collider2D>();
                if (col != null)
                    col.enabled = false;

                if (animator != null)
                    animator.SetTrigger("Attach");

                StartCoroutine(ApplyDot(enemy));
            }
        }
    }

    private IEnumerator ApplyDot(Enemy enemy)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            enemy.TakeDamage(damage);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        Destroy(gameObject);
    }
}
