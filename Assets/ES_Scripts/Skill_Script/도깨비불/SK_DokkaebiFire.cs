using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Projectile/도깨비불")]
public class SK_DokkaebiFire : SkillBase_ES
{
    [Header("Projectile Settings")]
    public string projectileKey = "DokkaebiFire"; // 풀 키
    public int count = 3;                          // 3발
    public float spreadDeg = 10f;                  // 좌/우 퍼짐 각도
    public float speed = 12f;
    public float life = 2.5f;                     // 초
    public int baseDamage = 10;

    [Header("Explosion")]
    public float explodeRadius = 1.6f;
    public LayerMask hitMask;                      // 적 레이어
    public string trailFxKey;                      // (선택) 탄 자취 FX
                                                   // impactKey(베이스 필드)는 폭발 FX로 사용

    public override void Execute(SkillContext ctx)
    {
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        float face = Mathf.Sign(ctx.caster.localScale.x);   // +1/-1
        float baseYaw = (face > 0f) ? 0f : 180f;               // 0° or 180°
        Vector2 origin = ctx.castPos;

        int shots = Mathf.Max(1, count);
        float[] angles = shots == 1 ? new float[] { 0f }
                                    : new float[] { +spreadDeg, 0f, -spreadDeg };

        for (int i = 0; i < shots; i++)
        {
            var go = ctx.SpawnFromPool?.Invoke(projectileKey);
            if (!go) continue;

            // 루트 스케일은 양수로 고정해 뒤집힘 영향 제거(안전)
            var ls = go.transform.localScale;
            go.transform.localScale = new Vector3(Mathf.Abs(ls.x), Mathf.Abs(ls.y), 1f);

            float spread = angles[Mathf.Min(i, angles.Length - 1)];

            // ★ 최종 회전(루트)과 이동방향(dir)을 동시에 계산
            float yaw = baseYaw + spread;
            Quaternion rootRot = Quaternion.AngleAxis(yaw, Vector3.forward);
            Vector2 dir = rootRot * Vector2.right; // ← 이동 방향 벡터

            // 위치/회전 세팅
            go.transform.SetPositionAndRotation(origin, rootRot);

            // (선택) 보이는 자식만 spread만 적용하고 flip 초기화
            var sr = go.GetComponentInChildren<SpriteRenderer>();
            if (sr)
            {
                var vls = sr.transform.localScale;
                sr.transform.localScale = new Vector3(Mathf.Abs(vls.x), Mathf.Abs(vls.y), 1f);
                sr.flipX = sr.flipY = false;
                sr.transform.localRotation = Quaternion.AngleAxis(spread, Vector3.forward);
            }

            var pj = go.GetComponent<ProjectileExplode2D>();
            if (!pj) continue;

            int dmg = Mathf.RoundToInt(baseDamage * powerByLevel.Evaluate(ctx.casterLevel) * TierMul());
            // ★ dir을 넘겨서 절대 transform.right에 의존하지 않음
            pj.Fire(ctx.caster, dmg, speed, life, hitMask, explodeRadius, impactKey, trailFxKey, dir);
        }
    }
}
