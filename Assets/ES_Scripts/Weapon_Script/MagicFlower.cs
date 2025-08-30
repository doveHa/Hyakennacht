using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFlower : MonoBehaviour
{
    public GameObject[] elementalProjectiles; // ��, ����, ���� ���� �߻�ü ������
    public float fireRate = 1.5f;
    public float detectionRadius = 5f;
    private int damage;

    public void SetDamage(int dmg) => damage = dmg;

    private void Start()
    {
        StartCoroutine(FireRoutine());
        Destroy(gameObject, 10f); // ��: 10�� �� ����
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            FireAtNearestEnemy();
        }
    }

    private void FireAtNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        Transform target = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    target = hit.transform;
                }
            }
        }

        if (target != null)
        {
            GameObject proj = Instantiate(GetRandomProjectile(), transform.position, Quaternion.identity);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (target.position - transform.position).normalized;
                rb.gravityScale = 0f;
                rb.linearVelocity = dir * 5f;
            }

            Projectile p = proj.GetComponent<Projectile>();
            if (p != null)
                p.SetDamage(damage);
        }
    }

    private GameObject GetRandomProjectile()
    {
        int index = Random.Range(0, elementalProjectiles.Length);
        return elementalProjectiles[index];
    }
}
