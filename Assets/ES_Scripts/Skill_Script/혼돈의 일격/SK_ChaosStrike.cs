using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AoE/ȥ���� �ϰ�")]
public class SK_ChaosStrike : SkillBase
{
    [Header("Wave/Crack Visual")]
    public string wavePoolKey = "CrackWave"; // Ǯ Ű(�������� ���� ������)
    public string sweepFxKey;                // (����) ���� FX
    public string hitFxKey;                  // (����) Ÿ�� FX(���� �ǰ� ����)

    [Header("Sweep Shape")]
    public float maxLength = 8f;   // ���� ���� ����
    public float width = 2.0f;     // �ڽ��� ��
    public float sweepDuration = 0.6f; // ����� �ð�(��)
    public float spawnOffset = 1.0f;

    [Header("Damage & CC")]
    public int primaryDamage = 60;     // ù ��(���� ����� ����) ���ʽ� ����
    public int sweepDamage = 30;       // �� �� �� ����(1ȸ)
    public float stunSeconds = 1.2f;   // ���� ���ӽð�
    public bool stunPrimaryOnly = false; // true�� ù ���� ����

    [Header("Filters")]
    public LayerMask enemyMask;       
    public string[] targetTags = new[] { "Enemy" };

    public override void Execute(SkillContext ctx)
    {
        // ��ٿ� ����
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        // ���� ����(��/��) ����: ��������Ʈ �������� +X��� ����
        float face = Mathf.Sign(ctx.caster.localScale.x);
        Vector2 forward = new Vector2(face, 0f);
        Vector2 origin = ctx.castPos;

        // ���� FX(����)
        if (!string.IsNullOrEmpty(sweepFxKey))
            ctx.PlayFXAt?.Invoke(sweepFxKey, origin);

        Vector2 spawnPos = origin + forward * spawnOffset;

        // ���̺� ������Ʈ ��ȯ
        var go = ctx.SpawnFromPool?.Invoke(wavePoolKey);
        if (!go) return;

        // ��ġ/���� ����
        go.transform.position = spawnPos;
        go.transform.right = forward;

        // �ٽ�: ���༺ ���� ������Ʈ ����
        var wave = go.GetComponent<CrackWave2D>();
        if (!wave) return;

        // ������ ������(����/Ƽ��)
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
