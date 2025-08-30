using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AoE/요기 폭발(짧은 밀침)")]
public class SK_YokiExplosion : SkillBase
{
    [Header("AoE")]
    public float radius = 2.0f;                 // 폭발 반경
    public int baseDamage = 8;                  // 소량 피해
    public LayerMask enemyMask;                 // 적 레이어
    public string[] targetTags = new[] { "Enemy" };

    [Header("Push")]
    public float pushBackDist = 3f;           // 뒤로 밀어낼 거리

    [Header("FX")]
    public bool useCasterCenter = true;         // 플레이어 중심

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

            // 데미지
            var enemy = co.GetComponentInParent<Enemy_ES>();
            if (enemy) enemy.TakeDamage(dmg);

            // 한 번만 뒤로 밀기
            Vector2 to = (Vector2)co.bounds.center - center;
            Vector2 dir = (to.sqrMagnitude > 0.0001f) ? to.normalized : Vector2.right;
            co.transform.position += (Vector3)(dir * pushBackDist);
        }

        if (!string.IsNullOrEmpty(impactKey))
            ctx.PlayFXAt?.Invoke(impactKey, center);
    }
}
