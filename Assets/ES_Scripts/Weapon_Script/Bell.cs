using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : MonoBehaviour
{
    public float destroyDelay = 0.5f; // 애니메이션 후 파괴 대기 시간
    private int damage;
    private Animator anim;
    private Rigidbody2D rb;
    private bool hasHit = false;

    public void SetDamage(int dmg) => damage = dmg;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 1.5f;

        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            hasHit = true;

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            if (anim != null)
                anim.SetTrigger("Hit");

            transform.localScale *= 2f;

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                rb.simulated = false;
            }

            StartCoroutine(DestroyAfterDelay());
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }

    public void SetDirection(float xDir)
    {
        StartCoroutine(ApplyForceAfterStart(xDir));
    }

    private IEnumerator ApplyForceAfterStart(float xDir)
    {
        yield return new WaitForFixedUpdate(); // physics update 이후까지 대기

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 force = new Vector2(xDir * 5f, 5f);
            rb.AddForce(force, ForceMode2D.Impulse);
            Debug.Log("Force applied (delayed): " + force);
        }

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (xDir < 0 ? -1 : 1);
        transform.localScale = scale;
    }
}
