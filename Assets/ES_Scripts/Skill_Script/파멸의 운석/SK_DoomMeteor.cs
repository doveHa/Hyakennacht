using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Meteor/파멸의 운석")]
public class SK_DoomMeteor : SkillBase
{
    [Header("Pool")]
    public string meteorKey = "DoomMeteor";   // 메테오 프리팹 키

    [Header("Burst")]
    public int count = 6;                     // 몇 개 떨어뜨릴지
    public float spreadRadius = 3.0f;         // 목표 중심 주변 랜덤 산포

    [Header("Spawn (relative to caster)")]
    public float spawnHeight = 10f;           // 캐릭터 위
    public float spawnAhead = 3.0f;          // 캐릭터 전방
    public float diagBias = 1.0f;          

    [Header("Physics")]
    public float fallSpeed = 18f;            
    public float gravityScale = 0f;          
    public float life = 5f;                   
    public LayerMask hitMask;                

    [Header("Damage")]
    public int baseImpactDamage = 90;         // 직격 데미지

    public override void Execute(SkillContext ctx)
    {
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        float face = Mathf.Sign(ctx.caster.localScale.x);
        Vector2 forward = new Vector2(face, 0f);

        Vector2 burstCenter = (Vector2)ctx.caster.position + Vector2.up * spawnHeight + forward * spawnAhead;

        Vector2 dir = (Vector2.down + forward * Mathf.Max(0f, diagBias)).normalized;
        Vector2 v0 = dir * Mathf.Max(0f, fallSpeed);

        int impactDmg = Mathf.RoundToInt(baseImpactDamage * powerByLevel.Evaluate(ctx.casterLevel) * TierMul());

        int c = Mathf.Max(1, count);
        for (int i = 0; i < c; i++)
        {
            var go = ctx.SpawnFromPool?.Invoke(meteorKey);
            if (!go) continue;

            // 스폰 위치: 중심 주변 랜덤 산포
            Vector2 offset = (spreadRadius > 0f) ? Random.insideUnitCircle * spreadRadius : Vector2.zero;
            Vector2 spawnPos = burstCenter + offset;

            // 페이로드 주입
            var pay = go.GetComponent<DoomMeteorPayload>() ?? go.AddComponent<DoomMeteorPayload>();
            pay.impactDamage = impactDmg;

            // 낙하체 발사
            var fp = go.GetComponent<FallingProjectile2D>();
            if (!fp) { Debug.LogWarning("[DoomMeteor] FallingProjectile2D missing."); continue; }

            fp.Fire(
                owner: ctx.caster,
                spawnPos: spawnPos,
                initialVelocity: v0,            // 동일한 대각선 방향으로 추락
                gravityScale: gravityScale,
                lifeSeconds: life,
                hitMask: hitMask,               // 적 레이어 
                spawnFxKey: spawnKey,          
                trailFxKey: null,               
                impactFxKey: impactKey,         
                fx: ctx.PlayFXAt
            );
        }
    }
}
