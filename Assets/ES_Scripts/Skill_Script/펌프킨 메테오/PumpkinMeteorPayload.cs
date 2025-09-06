using UnityEngine;

public class PumpkinMeteorPayload : MonoBehaviour, IFallPayload
{
    [Header("Impact Damage")]
    public int impactDamage = 0;             // �� SO���� ������ ����

    [Header("Fire Patch")]
    public string firePatchKey = "FirePatch";
    public float patchRadius = 2.0f;
    public float patchDuration = 5f;
    public int patchDps = 3;                 // "�� �� 3"
    public float tickInterval = 1.0f;
    public LayerMask enemyMask;
    public string[] targetTags = new[] { "Enemy" };
    public string tickFxKey;

    public void OnImpact(Transform owner, Vector2 hitPoint, Collider2D hit)
    {
        // 1) �浹 ��� ��� ����
        var enemy = hit.GetComponentInParent<AEnemyStats>();
        if (enemy && impactDamage > 0)
        {
            enemy.TakeDamage(impactDamage);
        }

        // 2) �浹 ������ �� ���� ����
        if (!owner) return;
        var caster = owner.GetComponent<SkillCaster>();
        if (caster == null || caster.pool == null) return;

        var patchGo = caster.pool.Spawn(firePatchKey);
        if (!patchGo) return;

        var patch = patchGo.GetComponent<FirePatch2D>();
        if (!patch) { Debug.LogWarning("[PumpkinMeteor] FirePatch2D missing."); return; }

        var fx = caster.fx != null ? caster.fx.PlayAt : (System.Action<string, Vector2>)null;

        patch.Arm(
            center: hitPoint,
            radius: patchRadius,
            duration: patchDuration,
            damagePerSecond: patchDps,
            tickInterval: tickInterval,
            enemyMask: enemyMask,
            targetTags: targetTags,
            tickFxKey: tickFxKey,
            fx: fx
        );
    }
}
