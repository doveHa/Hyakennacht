using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Projectile/�������")]
public class SK_DokkaebiFire : SkillBase_ES
{
    [Header("Projectile Settings")]
    public string projectileKey = "DokkaebiFire"; // Ǯ Ű
    public int count = 3;                          // 3��
    public float spreadDeg = 10f;                  // ��/�� ���� ����
    public float speed = 12f;
    public float life = 2.5f;                     // ��
    public int baseDamage = 10;

    [Header("Explosion")]
    public float explodeRadius = 1.6f;
    public LayerMask hitMask;                      // �� ���̾�
    public string trailFxKey;                      // (����) ź ���� FX
                                                   // impactKey(���̽� �ʵ�)�� ���� FX�� ���

    public override void Execute(SkillContext ctx)
    {
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        float face = Mathf.Sign(ctx.caster.localScale.x);   // +1/-1
        float baseYaw = (face > 0f) ? 0f : 180f;               // 0�� or 180��
        Vector2 origin = ctx.castPos;

        int shots = Mathf.Max(1, count);
        float[] angles = shots == 1 ? new float[] { 0f }
                                    : new float[] { +spreadDeg, 0f, -spreadDeg };

        for (int i = 0; i < shots; i++)
        {
            var go = ctx.SpawnFromPool?.Invoke(projectileKey);
            if (!go) continue;

            // ��Ʈ �������� ����� ������ ������ ���� ����(����)
            var ls = go.transform.localScale;
            go.transform.localScale = new Vector3(Mathf.Abs(ls.x), Mathf.Abs(ls.y), 1f);

            float spread = angles[Mathf.Min(i, angles.Length - 1)];

            // �� ���� ȸ��(��Ʈ)�� �̵�����(dir)�� ���ÿ� ���
            float yaw = baseYaw + spread;
            Quaternion rootRot = Quaternion.AngleAxis(yaw, Vector3.forward);
            Vector2 dir = rootRot * Vector2.right; // �� �̵� ���� ����

            // ��ġ/ȸ�� ����
            go.transform.SetPositionAndRotation(origin, rootRot);

            // (����) ���̴� �ڽĸ� spread�� �����ϰ� flip �ʱ�ȭ
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
            // �� dir�� �Ѱܼ� ���� transform.right�� �������� ����
            pj.Fire(ctx.caster, dmg, speed, life, hitMask, explodeRadius, impactKey, trailFxKey, dir);
        }
    }
}
