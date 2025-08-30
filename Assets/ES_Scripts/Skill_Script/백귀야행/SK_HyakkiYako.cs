using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Summon/��;���(������)")]
public class SK_HyakkiYako : SkillBase
{
    [Header("Pools & VFX")]
    public string spiritKey = "YokaiSpirit"; // ȥ�� ������ Ǯ Ű
    public string fogSpawnFxKey;             // ��ȯ �� �� ���� FX

    [Header("Lifetime")]
    public float duration = 6f;

    [Header("Counts")]
    public int combatCount = 5;              // �� ���� �� ������
    public bool singleWhenNoEnemy = true;    // �� ������ 1������

    [Header("Spawn/Idle (feet semicircle)")]
    public float footYOffset = -0.2f;        // �� ��ġ ����
    public float spawnRadius = 1.0f;         // ���� ���� �ݰ�
    public float idleSlotRadius = 0.8f;      // ��� ���� �ݰ�(�Ʒ� �ݿ�)
    [Range(0, 360)] public float spawnArcStartDeg = 200f;
    [Range(0, 360)] public float spawnArcEndDeg = 340f;

    [Header("Detect/Chase")]
    public float detectRadius = 6f;
    public LayerMask enemyMask;
    public string[] targetTags = new[] { "Enemy" };

    [Header("Move")]
    public float moveSpeed = 6f;
    public float turnLerp = 12f;
    public float standoffDistance = 0.55f;   // ���� �ּҰŸ�

    [Header("Attack")]
    public int damage = 10;
    public float attackCooldown = 0.6f;
    public string hitFxKey;
    public float ignoreSameTargetSeconds = 0.35f;

    public override void Execute(SkillContext ctx)
    {
        // ��Ÿ�� ����
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        // ��Ʈ�ѷ��� ������ �߰�, ������ ����
        var ctrl = ctx.caster.GetComponent<HyakkiController2D>();
        if (!ctrl) ctrl = ctx.caster.gameObject.AddComponent<HyakkiController2D>();

        // �� ���� FX(����)
        if (!string.IsNullOrEmpty(fogSpawnFxKey))
            ctx.PlayFXAt?.Invoke(fogSpawnFxKey, ctx.castPos + new Vector2(0f, footYOffset));

        // ����/Ƽ�� ������ ����
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
