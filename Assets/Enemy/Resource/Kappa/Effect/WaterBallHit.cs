using Manager;
using UnityEngine;

public class WaterBallHit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            Destroy(gameObject);
        }
        else if (other.gameObject.tag.Equals("Enemy"))
        {
        }
        else
        {
            Destroy(gameObject);
        }
    }
}