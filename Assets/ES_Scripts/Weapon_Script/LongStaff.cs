using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongStaff : MonoBehaviour
{
    public float destroyTime = 0;

    private float amplitude = 0.5f;
    private float frequency = 3f;
    private Vector2 direction;
    private float speed;
    private float time;
    private int damage;

    private Vector2 startPos;
    private Transform target;

    private void Start()
    {
        Debug.Log("파괴 실행");
        Destroy(gameObject, destroyTime);
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public void Launch(Vector2 dir, float speed)
    {
        this.direction = dir.normalized;
        this.speed = speed;
        startPos = transform.position;
    }

    private void Update()
    {
        time += Time.deltaTime;

        // Soft Homing
        if (target == null)
            target = FindClosestEnemy();

        if (target != null)
        {
            Vector2 toTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;
            direction = Vector2.Lerp(direction, toTarget, 0.02f); // 부드럽게 방향 보정
        }

        Vector2 offset = new Vector2(0, Mathf.Sin(time * frequency) * amplitude);
        transform.position = startPos + direction * speed * time + offset;
    }

    private Transform FindClosestEnemy()
    {
        float minDist = 10f;
        Transform closest = null;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            float dist = Vector2.Distance(transform.position, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj.transform;
            }
        }
        return closest;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
