using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PotionEffectType
{
    Heal,
    Poison,
    Fire,
    Hit,
    Death
}

public static class PotionEffectHelper 
{
    public static PotionEffectType GetRandomEffect()
    {
        float rand = Random.Range(0f, 100f);

        if (rand < 15f) return PotionEffectType.Heal;
        if (rand < 43f) return PotionEffectType.Poison; // 15~43
        if (rand < 71f) return PotionEffectType.Fire;   // 43~71
        if (rand < 99f) return PotionEffectType.Hit;    // 71~99
        return PotionEffectType.Death;            // 99~100
    }
}
