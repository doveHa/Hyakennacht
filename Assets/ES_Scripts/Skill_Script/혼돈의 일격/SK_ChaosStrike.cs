using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AoE/혼돈의 일격")]
public class SK_ChaosStrike : SkillBase
{
    [Header("Wave/Crack Visual")]
    public string wavePoolKey = "CrackWave"; // 풀 키(갈라지는 지면 프리팹)
    public string sweepFxKey;                // (선택) 시작 FX
    public string hitFxKey;                  // (선택) 타격 FX(개별 피격 지점)

    [Header("Sweep Shape")]
    public float maxLength = 8f;   // 최종 도달 길이
    public float width = 2.0f;     // 박스의 폭
    public float sweepDuration = 0.6f; // 쓸어가는 시간(초)
    public float spawnOffset = 1.0f;

    [Header("Damage & CC")]
    public int primaryDamage = 60;     // 첫 적(가장 가까운 전방) 보너스 피해
    public int sweepDamage = 30;       // 그 외 적 피해(1회)
    public float stunSeconds = 1.2f;   // 스턴 지속시간
    public bool stunPrimaryOnly = false; // true면 첫 적만 스턴

    [Header("Filters")]
    public LayerMask enemyMask;       
    public string[] targetTags = new[] { "Enemy" };

    public override void Execute(SkillContext ctx)
    {
        // 쿨다운 시작
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        // 전방 벡터(좌/우) 고정: 스프라이트 오른쪽이 +X라고 가정
        float face = Mathf.Sign(ctx.caster.localScale.x);
        Vector2 forward = new Vector2(face, 0f);
        Vector2 origin = ctx.castPos;

        // 시작 FX(선택)
        if (!string.IsNullOrEmpty(sweepFxKey))
            ctx.PlayFXAt?.Invoke(sweepFxKey, origin);

        Vector2 spawnPos = origin + forward * spawnOffset;

        // 웨이브 오브젝트 소환
        var go = ctx.SpawnFromPool?.Invoke(wavePoolKey);
        if (!go) return;

        // 위치/방향 세팅
        go.transform.position = spawnPos;
        go.transform.right = forward;

        // 핵심: 진행성 스윕 컴포넌트 무장
        var wave = go.GetComponent<CrackWave2D>();
        if (!wave) return;

        // 데미지 스케일(레벨/티어)
        int priDmg = Mathf.RoundToInt(primaryDamage * powerByLevel.Evaluate(ctx.casterLevel) * TierMul());
        int swpDmg = Mathf.RoundToInt(sweepDamage * powerByLevel.Evaluate(ctx.casterLevel) * TierMul());

        wave.Arm(
            owner: ctx.caster,
            origin: origin,
            forward: forward,
            maxLength: maxLength,
            width: width,
            duration: sweepDuration,
            primaryDamage: priDmg,
            sweepDamage: swpDmg,
            stunSeconds: stunSeconds,
            stunPrimaryOnly: stunPrimaryOnly,
            enemyMask: enemyMask,
            targetTags: targetTags,
            hitFxKey: hitFxKey,
            fx: ctx.PlayFXAt
        );
    }
}
