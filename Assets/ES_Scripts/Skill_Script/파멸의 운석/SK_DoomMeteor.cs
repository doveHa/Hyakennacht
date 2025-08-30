using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Meteor/�ĸ��� �")]
public class SK_DoomMeteor : SkillBase
{
    [Header("Pool")]
    public string meteorKey = "DoomMeteor";   // ���׿� ������ Ű

    [Header("Burst")]
    public int count = 6;                     // �� �� ����߸���
    public float spreadRadius = 3.0f;         // ��ǥ �߽� �ֺ� ���� ����

    [Header("Spawn (relative to caster)")]
    public float spawnHeight = 10f;           // ĳ���� ��
    public float spawnAhead = 3.0f;          // ĳ���� ����
    public float diagBias = 1.0f;          

    [Header("Physics")]
    public float fallSpeed = 18f;            
    public float gravityScale = 0f;          
    public float life = 5f;                   
    public LayerMask hitMask;                

    [Header("Damage")]
    public int baseImpactDamage = 90;         // ���� ������

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

            // ���� ��ġ: �߽� �ֺ� ���� ����
            Vector2 offset = (spreadRadius > 0f) ? Random.insideUnitCircle * spreadRadius : Vector2.zero;
            Vector2 spawnPos = burstCenter + offset;

            // ���̷ε� ����
            var pay = go.GetComponent<DoomMeteorPayload>() ?? go.AddComponent<DoomMeteorPayload>();
            pay.impactDamage = impactDmg;

            // ����ü �߻�
            var fp = go.GetComponent<FallingProjectile2D>();
            if (!fp) { Debug.LogWarning("[DoomMeteor] FallingProjectile2D missing."); continue; }

            fp.Fire(
                owner: ctx.caster,
                spawnPos: spawnPos,
                initialVelocity: v0,            // ������ �밢�� �������� �߶�
                gravityScale: gravityScale,
                lifeSeconds: life,
                hitMask: hitMask,               // �� ���̾� 
                spawnFxKey: spawnKey,          
                trailFxKey: null,               
                impactFxKey: impactKey,         
                fx: ctx.PlayFXAt
            );
        }
    }
}
