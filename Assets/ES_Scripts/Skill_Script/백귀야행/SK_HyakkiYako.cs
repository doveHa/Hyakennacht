using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Summon/归蓖具青(包府屈)")]
public class SK_HyakkiYako : SkillBase
{
    [Header("Pools & VFX")]
    public string spiritKey = "YokaiSpirit";
    public string fogSpawnFxKey;

    [Header("Lifetime")]
    public float duration = 6f;

    [Header("Counts")]
    public int combatCount = 5;
    public bool singleWhenNoEnemy = true;

    [Header("Spawn/Idle (feet semicircle)")]
    public float footYOffset = -0.2f;
    public float spawnRadius = 1.0f;
    public float idleSlotRadius = 0.8f;
    [Range(0, 360)] public float spawnArcStartDeg = 200f;
    [Range(0, 360)] public float spawnArcEndDeg = 340f;

    [Header("Detect/Chase")]
    public float detectRadius = 6f;
    public LayerMask enemyMask;
    public string[] targetTags = new[] { "Enemy" };

    [Header("Ignore (name contains)")]
    public string[] ignoreNameContains = new[] { "SmallStraw" };

    [Header("Move")]
    public float moveSpeed = 6f;
    public float turnLerp = 12f;
    public float standoffDistance = 0.55f;

    [Header("Attack")]
    public int damage = 10;
    public float attackCooldown = 0.6f;
    public string hitFxKey;
    public float ignoreSameTargetSeconds = 0.35f;

    public override void Execute(SkillContext ctx)
    {
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        var ctrl = ctx.caster.GetComponent<HyakkiController2D>();
        if (!ctrl) ctrl = ctx.caster.gameObject.AddComponent<HyakkiController2D>();

        if (!string.IsNullOrEmpty(fogSpawnFxKey))
            ctx.PlayFXAt?.Invoke(fogSpawnFxKey, ctx.castPos + new Vector2(0f, footYOffset));

        int dmg = Mathf.RoundToInt(damage * powerByLevel.Evaluate(ctx.casterLevel) * TierMul());

        ctrl.Arm(new HyakkiController2D.Config
        {
            poolSpawn = ctx.SpawnFromPool,
            playFxAt = ctx.PlayFXAt,

            spiritKey = spiritKey,
            totalDuration = duration,

            combatCount = Mathf.Max(1, combatCount),
            singleWhenNoEnemy = singleWhenNoEnemy,

            footYOffset = footYOffset,
            spawnRadius = spawnRadius,
            idleSlotRadius = idleSlotRadius,
            arcStartDeg = spawnArcStartDeg,
            arcEndDeg = spawnArcEndDeg,

            detectRadius = detectRadius,
            enemyMask = enemyMask,
            targetTags = targetTags,
            ignoreNameContains = ignoreNameContains,   

            moveSpeed = moveSpeed,
            turnLerp = turnLerp,
            standoffDistance = standoffDistance,

            damage = dmg,
            attackCooldown = attackCooldown,
            hitFxKey = hitFxKey,
            ignoreSameTargetSeconds = ignoreSameTargetSeconds
        });
    }
}
