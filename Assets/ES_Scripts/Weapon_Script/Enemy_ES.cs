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
        Debug.Log($"{gameObject.name}이(가) {dmg} 데미지를 입었습니다. 남은 체력: {hp}");

        if (hp <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        hp += amount;
        hp = Mathf.Min(hp, maxHp); 
        Debug.Log($"적이 {amount}만큼 회복됨");
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name}이(가) 사망했습니다.");
        Destroy(gameObject);
    }
}
