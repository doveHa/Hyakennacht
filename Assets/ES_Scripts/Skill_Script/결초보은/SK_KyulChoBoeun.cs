using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Trap/���ʺ���")]
public class SK_KyulChoBoeun : SkillBase
{
    [Header("Trap Pool Key")]
    public string trapKey = "KyulTrap";

    [Header("Placement")]
    public float offset = 1.0f;          // ĳ���� �� ��ġ
    public float feetYOffset = -0.2f;    // �� ���� ����
    public bool alignToFacing = true;    // �¿� ȸ�� ����

    [Header("Limits")]
    public int maxActivePerCaster = 5;   // ���ÿ� �ִ� ����
    public float trapLifetime = 20f;     // �� ������ �ڵ� ȸ��

    [Header("Effect")]
    public int baseDamage = 20;
    public float stunSeconds = 1.5f;     // Stunnable�� ����Ǵ� �ð�
    public LayerMask enemyMask;
    public string[] targetTags = new[] { "Enemy" };
    public string hitFxKey;              // ���� ���� �� FX

    // ĳ���ͺ� ���� ��ġ ���(������ �ͺ��� ����)
    static readonly Dictionary<int, LinkedList<PooledObject>> _activeByCaster = new();

    public override void Execute(SkillContext ctx)
    {
        // ��Ÿ��
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        // ���� ��ġ
        float face = Mathf.Sign(ctx.caster.localScale.x);
        Vector2 forward = new Vector2(face, 0f);
        Vector2 origin = ctx.castPos + new Vector2(0f, feetYOffset);
        Vector2 spawnPos = origin + forward * Mathf.Max(0.05f, offset);

        // Ǯ ����
        var go = ctx.SpawnFromPool?.Invoke(trapKey);
        if (!go) return;

        go.transform.position = spawnPos;
        if (alignToFacing) go.transform.right = forward;

        var trap = go.GetComponent<TrapPlant2D>();
        if (!trap)
        {
            Debug.LogWarning("[KyulChoBoeun] TrapPlant2D missing on prefab.");
            return;
        }

        // ĳ���ͺ� ť �غ�
        int casterKey = ctx.caster.GetInstanceID();
        if (!_activeByCaster.TryGetValue(casterKey, out var list))
        {
            list = new LinkedList<PooledObject>();
            _activeByCaster[casterKey] = list;
        }

        // PooledObject ��ū Ȯ��
        var token = go.GetComponent<PooledObject>();
        if (!token) token = go.AddComponent<PooledObject>(); // ������ ����
        token.key = trapKey;  // (����) Ű ����ȭ

        // �ʰ� �� ���� ������ �� ȸ��
        int cap = Mathf.Max(1, maxActivePerCaster);
        if (list.Count >= cap)
        {
            var oldest = list.First.Value;
            list.RemoveFirst();
            if (oldest) oldest.ReturnToPool();
        }

        // �� �� ���
        list.AddLast(token);

        // �����ϸ�
        int dmg = Mathf.RoundToInt(baseDamage * powerByLevel.Evaluate(ctx.casterLevel) * TierMul());

        // ����
        trap.Arm(
            damage: dmg,
            stunSeconds: stunSeconds,
            enemyMask: enemyMask,
            targetTags: targetTags,
            hitFxKey: hitFxKey,
            playFxAt: ctx.PlayFXAt,
            onFinish: (t) =>
            {
                // ȸ�� �� ��Ͽ��� ����(���� Ž��)
                var node = list.First;
                while (node != null)
                {
                    var next = node.Next;
                    if (node.Value == token) { list.Remove(node); break; }
                    node = next;
                }
            },
            autoDespawnSeconds: trapLifetime
        );
    }
}
