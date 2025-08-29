using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    private int damage;
    private float interval;
    private float timer;
    private int ticksRemaining;
    private string effectType;

    public void Initialize(int damage, float interval, string effectType = "", int ticks = 3)
    {
        this.damage = damage;
        this.interval = interval;
        this.effectType = effectType;
        this.ticksRemaining = ticks;
    }

    private void Update()
    {
        if (ticksRemaining <= 0)
        {
            Destroy(this);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            ticksRemaining--;

            GetComponent<Enemy_ES>()?.TakeDamage(damage);

            if (effectType == "Electric")
            {
                GameObject fx = Resources.Load<GameObject>("Effects/Ãæ°Ý");
                if (fx != null)
                    Instantiate(fx, transform.position, Quaternion.identity);
            }
        }
    }
}
