using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AoE/��� ����(ª�� ��ħ)")]
public class SK_YokiExplosion : SkillBase
{
    [Header("AoE")]
    public float radius = 2.0f;                 // ���� �ݰ�
    public int baseDamage = 8;                  // �ҷ� ����
    public LayerMask enemyMask;                 // �� ���̾�
    public string[] targetTags = new[] { "Enemy" };

    [Header("Push")]
    public float pushBackDist = 3f;           // �ڷ� �о �Ÿ�

    [Header("FX")]
    public bool useCasterCenter = true;         // �÷��̾� �߽�

    public override void Execute(SkillContext ctx)
    {
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        Vector2 center = useCasterCenter ? (Vector2)ctx.caster.position : ctx.castPos;

        if (!string.IsNullOrEmpty(spawnKey))
            ctx.PlayFXAt?.Invoke(spawnKey, center);

        Collider2D[] buf = new Collider2D[48];
        int n = Phys2DCompat.OverlapCircle(center, radius, buf, enemyMask, includeTriggers: true);
        for (int i = 0; i < n; i++) Debug.Log($" - {buf[i].name} (layer={buf[i].gameObject.layer})");

        int dmg = Mathf.RoundToInt(baseDamage * powerByLevel.Evaluate(ctx.casterLevel) * TierMul());

        for (int i = 0; i < n; i++)
        {
            var co = buf[i];
            if (!co) continue;

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

            // ������
            var enemy = co.GetComponentInParent<AEnemyStats>();
            if (enemy) enemy.TakeDamage(dmg);

            // �� ���� �ڷ� �б�
            Vector2 to = (Vector2)co.bounds.center - center;
            Vector2 dir = (to.sqrMagnitude > 0.0001f) ? to.normalized : Vector2.right;
            co.transform.position += (Vector3)(dir * pushBackDist);
        }

        if (!string.IsNullOrEmpty(impactKey))
            ctx.PlayFXAt?.Invoke(impactKey, center);
    }
}
