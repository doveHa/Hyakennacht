using Manager;
using UnityEngine;

public class HeartPlusHealth : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            SystemManager.Manager.hpControl.PlusHp();
        }
    }
}
