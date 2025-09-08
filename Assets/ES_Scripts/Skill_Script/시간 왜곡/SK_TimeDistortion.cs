using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AoE/�ð� �ְ�")]
public class SK_TimeDistortion : SkillBase
{
    [Header("AoE")]
    public float radius = 3.5f;                  // ����
    public LayerMask enemyMask;
    public string[] targetTags = new[] { "Enemy" };

    [Header("Stun")]
    public float stunSeconds = 1.5f;             // ���� ���� �ð�

    [Header("Clock Marker")]
    public string clockPoolKey = "ClockMarker";  // ������Ʈ Ǯ Ű
    public Vector2 clockOffset = new Vector2(0f, 1.5f); // �Ӹ� �� ��ġ

    public override void Execute(SkillContext ctx)
    {
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        Vector2 center = ctx.caster.position;

        Collider2D[] buf = new Collider2D[64];
        int n = Phys2DCompat.OverlapCircle(center, radius, buf, enemyMask, includeTriggers: true);

        int dmg = Mathf.RoundToInt(powerByLevel.Evaluate(ctx.casterLevel) * TierMul());

        for (int i = 0; i < n; i++)
        {
            var co = buf[i];
            if (!co) continue;

            // �±� ����
            if (targetTags != null && targetTags.Length > 0)
            {
                bool ok = false;
                for (int t = 0; t < targetTags.Length; t++)
                {
                    string tg = targetTags[t];
                    if (!string.IsNullOrEmpty(tg) && (co.CompareTag(tg) || co.transform.root.CompareTag(tg)))
                    { ok = true; break; }
                }
                if (!ok) continue;
            }

            var enemy = co.GetComponentInParent<AEnemyStats>();
            if (!enemy) continue;

            var stun = enemy.GetComponent<Stunnable>();
            if (!stun) stun = enemy.gameObject.AddComponent<Stunnable>();
            stun.ApplyStun(stunSeconds);

            var go = ctx.SpawnFromPool?.Invoke(clockPoolKey);
            if (go)
            {
                go.transform.position = (Vector2)enemy.transform.position + clockOffset;
                var marker = go.GetComponent<ClockMarker>();
                if (marker)
                {
                    marker.Arm(enemy.transform, clockOffset, stunSeconds);
                }
            }
        }
    }
}
