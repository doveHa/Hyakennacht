using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    public float duration = 3f; // ���� �ð�
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
            Enemy_ES enemy = other.GetComponent<Enemy_ES>();
            if (enemy != null)
            {
                target = other.transform;
                transform.SetParent(target);
                transform.localPosition = Vector3.zero;

                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
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

    private IEnumerator ApplyDot(Enemy_ES enemy)
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
