using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Summon/백귀야행(관리형)")]
public class SK_HyakkiYako : SkillBase
{
    [Header("Pools & VFX")]
    public string spiritKey = "YokaiSpirit"; // 혼령 프리팹 풀 키
    public string fogSpawnFxKey;             // 소환 시 발 연무 FX

    [Header("Lifetime")]
    public float duration = 6f;

    [Header("Counts")]
    public int combatCount = 5;              // 적 있을 때 마릿수
    public bool singleWhenNoEnemy = true;    // 적 없으면 1마리만

    [Header("Spawn/Idle (feet semicircle)")]
    public float footYOffset = -0.2f;        // 발 위치 보정
    public float spawnRadius = 1.0f;         // 최초 스폰 반경
    public float idleSlotRadius = 0.8f;      // 대기 슬롯 반경(아래 반원)
    [Range(0, 360)] public float spawnArcStartDeg = 200f;
    [Range(0, 360)] public float spawnArcEndDeg = 340f;

    [Header("Detect/Chase")]
    public float detectRadius = 6f;
    public LayerMask enemyMask;
    public string[] targetTags = new[] { "Enemy" };

    [Header("Move")]
    public float moveSpeed = 6f;
    public float turnLerp = 12f;
    public float standoffDistance = 0.55f;   // 적과 최소거리

    [Header("Attack")]
    public int damage = 10;
    public float attackCooldown = 0.6f;
    public string hitFxKey;
    public float ignoreSameTargetSeconds = 0.35f;

    public override void Execute(SkillContext ctx)
    {
        // 쿨타임 시작
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        // 컨트롤러가 없으면 추가, 있으면 갱신
        var ctrl = ctx.caster.GetComponent<HyakkiController2D>();
        if (!ctrl) ctrl = ctx.caster.gameObject.AddComponent<HyakkiController2D>();

        // 발 연무 FX(선택)
        if (!string.IsNullOrEmpty(fogSpawnFxKey))
            ctx.PlayFXAt?.Invoke(fogSpawnFxKey, ctx.castPos + new Vector2(0f, footYOffset));

        // 레벨/티어 스케일 적용
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
