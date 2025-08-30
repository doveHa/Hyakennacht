using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Meteor/펌프킨 메테오")]
public class SK_PumpkinMeteor : SkillBase
{
    [Header("Pools")]
    public string meteorKey = "PumpkinMeteor";
    public string firePatchKey = "FirePatch";

    [Header("Drop (relative to caster)")]
    public float spawnHeight = 8f;
    public float spawnAhead = 2.5f;
    public float diagBias = 1.0f;

    [Header("Physics")]
    public float fallSpeed = 14f;
    public float gravityScale = 0f;
    public float life = 4f;
    public LayerMask hitMask;          

    [Header("Impact Damage")]
    public int baseImpactDamage = 20;  // 충돌 즉시 피해 기본값

    [Header("Fire Patch")]
    public float patchRadius = 2.0f;
    public float patchDuration = 5f;
    public int patchDps = 3;
    public float tickInterval = 1f;
    public LayerMask patchEnemyMask;
    public string[] patchTargetTags = new[] { "Enemy" };
    public string tickFxKey;

    public override void Execute(SkillContext ctx)
    {
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        float face = Mathf.Sign(ctx.caster.localScale.x);
        Vector2 forward = new Vector2(face, 0f);
        Vector2 spawnPos = (Vector2)ctx.caster.position + Vector2.up * spawnHeight + forward * spawnAhead;

        Vector2 dir = (Vector2.down + forward * Mathf.Max(0f, diagBias)).normalized;
        Vector2 v0 = dir * Mathf.Max(0f, fallSpeed);

        var go = ctx.SpawnFromPool?.Invoke(meteorKey);
        if (!go) return;

        int impactDmg = Mathf.RoundToInt(baseImpactDamage * powerByLevel.Evaluate(ctx.casterLevel) * TierMul());

        // 페이로드 세팅
        var pay = go.GetComponent<PumpkinMeteorPayload>() ?? go.AddComponent<PumpkinMeteorPayload>();
        pay.impactDamage = impactDmg;        
        pay.firePatchKey = firePatchKey;
        pay.patchRadius = patchRadius;
        pay.patchDuration = patchDuration;
        pay.patchDps = patchDps;
        pay.tickInterval = tickInterval;
        pay.enemyMask = patchEnemyMask;
        pay.targetTags = patchTargetTags;
        pay.tickFxKey = tickFxKey;

        var fp = go.GetComponent<FallingProjectile2D>();
        if (!fp) { Debug.LogWarning("[PumpkinMeteor] FallingProjectile2D missing."); return; }

        fp.Fire(
            owner: ctx.caster,
            spawnPos: spawnPos,
            initialVelocity: v0,
            gravityScale: gravityScale,
            lifeSeconds: life,
            hitMask: hitMask,        
            spawnFxKey: spawnKey,
            trailFxKey: null,
            impactFxKey: impactKey,
            fx: ctx.PlayFXAt
        );
    }
}
