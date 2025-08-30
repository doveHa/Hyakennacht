using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AoE/Â÷¿øÀÇÆ´")]
public class SK_DimensionalRift : SkillBase
{
    [Header("Rift Settings")]
    public string riftPoolKey = "DimRift";
    public float duration = 4.0f;
    public float offset = 1.5f;
    public float radius = 2.0f;
    [Range(0f, 180f)] public float frontArcDeg = 120f;
    public int dps = 12;
    public float tickInterval = 0.25f;
    public float pullForce = 18f;
    public float maxPullSpeed = 8f;
    public LayerMask physicsMask;
    public string[] targetTags = new[] { "Enemy" };

    public override void Execute(SkillContext ctx)
    {
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        float face = Mathf.Sign(ctx.caster.localScale.x);
        Vector2 forward = new Vector2(face, 0f);   // Ç×»ó ÁÂ/¿ì·Î¸¸

        Vector2 spawnPos = (Vector2)ctx.caster.position + forward * Mathf.Max(0.1f, offset);

        /*
        if (!string.IsNullOrEmpty(spawnKey))
            ctx.PlayFXAt?.Invoke(spawnKey, spawnPos);
        */

        var go = ctx.SpawnFromPool?.Invoke(riftPoolKey);
        if (!go) return;

        go.transform.position = spawnPos;
        go.transform.right = forward;

        var rift = go.GetComponent<RiftZone2D>();
        if (!rift) return;

        int tickDamage = Mathf.RoundToInt(dps * powerByLevel.Evaluate(ctx.casterLevel) * TierMul());
        rift.Arm(
            owner: ctx.caster,
            duration: duration,
            radius: radius,
            frontArcDeg: frontArcDeg,
            tickDamagePerSec: tickDamage,
            tickInterval: tickInterval,
            pullForce: pullForce,
            maxPullSpeed: maxPullSpeed,
            mask: physicsMask,
            impactFxKey: impactKey,
            fx: ctx.PlayFXAt,
            targetTags: targetTags
        );
    }
}
