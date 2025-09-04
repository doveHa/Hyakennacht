using System.Collections;
using Manager;
using UnityEngine;

public class WaterBallHit : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            Destroy(gameObject);
        }else if (other.gameObject.tag.Equals("Enemy") || other.gameObject.tag.Equals("Projectile"))
        {
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
}