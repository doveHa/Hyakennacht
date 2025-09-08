using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public float destroyTime = 2f; // �ڵ� �ı� �ð�
    private int damage;

    public void SetDamage(int dmg) => damage = dmg;

    private void Start()
    {
        Debug.Log("Ʈ�� ������, �ı� �����");
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            AEnemyStats aEnemy = other.GetComponent<AEnemyStats>();
            if (aEnemy != null)
            {
                aEnemy.TakeDamage(damage);
                Debug.Log($"Ʈ�� ����! {other.name}���� {damage} ������");
            }

            Destroy(gameObject);
        }
    }
}
