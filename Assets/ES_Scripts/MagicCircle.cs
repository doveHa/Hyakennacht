using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircle : MonoBehaviour
{
    private int damage;

    public void SetDamage(int dmg) => damage = dmg;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            ES.Enemy enemy = other.GetComponent<ES.Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(Mathf.RoundToInt(damage * Time.deltaTime));
            }
        }
    }
}
