using System;
using Manager;
using UnityEngine;

public class BallHit : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            SystemManager.Manager.hpControl.MinusHp();
            Destroy(gameObject);
        }
    }
}
