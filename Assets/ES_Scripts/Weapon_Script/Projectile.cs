using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private string weaponName;

    public float destroyTime = 0;
    private int damage;

    public void SetWeaponName(string name) => weaponName = name;
    public void SetDamage(int dmg) => damage = dmg;

    private void Start()
    {
        Debug.Log("�ı� ����");
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log(other.name);
            other.GetComponentInParent<EnemyStats>()?.TakeDamage(damage);
            Debug.Log("�߻� �Ϸ�");

            if (weaponName != "����") 
            {
                Destroy(gameObject);
            }
        }
    }
}
