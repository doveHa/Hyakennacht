using Manager;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            Debug.Log("Enemy");
            SystemManager.Manager.HpControl.MinusHp();
        }
    }
}
