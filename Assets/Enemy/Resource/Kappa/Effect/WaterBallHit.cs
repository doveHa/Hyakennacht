using System.Collections;
using Manager;
using UnityEngine;

public class WaterBallHit : MonoBehaviour
{
    public float cooldown = 20f;
    private float startTime;

    void Update()
    {
        if (startTime + cooldown < Time.time)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            Destroy(gameObject);
        }
    }
    
    void OnEnable()
    {
        startTime = Time.time;
    }
}