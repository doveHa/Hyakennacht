using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ES : MonoBehaviour
{
    public int hp = 10;
    public int maxHp = 20;
    public float moveSpeed = 10;

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        Debug.Log($"{gameObject.name}��(��) {dmg} �������� �Ծ����ϴ�. ���� ü��: {hp}");

        if (hp <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        hp += amount;
        hp = Mathf.Min(hp, maxHp); 
        Debug.Log($"���� {amount}��ŭ ȸ����");
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name}��(��) ����߽��ϴ�.");
        Destroy(gameObject);
    }
}
