using UnityEngine;

public enum Faction { Yokai, Witch }
public enum Tier { Low, Mid, High }
public enum Exec { Projectile, AoEInstant, AoEDelayed, Dash, Buff, Debuff, TrapTotem, Summon, Channel, Passive }
public enum Targeting { None, Self, Direction, Point, Enemy }

public abstract class SkillBase_ES : ScriptableObject
{
    [Header("ID & Meta")]
    public string skillId;
    public string displayName;
    public Faction factionGate;
    public Tier tier;
    public Exec exec;
    public Targeting targeting;

    [Header("Rules")]
    public float cooldown = 6f;     // 쿨타임만 사용
    public float castTime = 0f;     // 선딜(초)
    public float range = 6f;
    public bool useGCD = false;

    /*
    [Header("Scaling")]
    public AnimationCurve powerByLevel = AnimationCurve.Linear(1, 1, 20, 3);
    public float tierMulLow = 0.85f, tierMulMid = 1.0f, tierMulHigh = 1.2f;
    레벨 스케일링
    */

    [Header("Scaling")]
    public AnimationCurve powerByLevel = AnimationCurve.Linear(1, 1, 20, 1);
    public float tierMulLow = 1.0f, tierMulMid = 1.0f, tierMulHigh = 1.0f;

    [Header("VFX/SFX Keys")]
    public string spawnKey;
    public string impactKey;

    public virtual bool CanCast(SkillContext ctx)
    {
        if (ctx == null) return false;

        if (ctx.IsOffCooldown != null && !ctx.IsOffCooldown.Invoke(skillId))
            return false;

        if (!CheckTargeting(ctx)) return false;
        if (!CheckRange(ctx)) return false;

        return true;
    }

    protected virtual bool CheckTargeting(SkillContext ctx)
    {
        switch (targeting)
        {
            case Targeting.Direction: return ctx.aimDir != Vector2.zero;
            case Targeting.Point: return true;
            case Targeting.Enemy: return ctx.target != null;
            default: return true;
        }
    }

    protected virtual bool CheckRange(SkillContext ctx)
    {
        if (range <= 0f) return true;
        switch (targeting)
        {
            case Targeting.Point:
                return Vector2.Distance(ctx.castPos, ctx.aimPoint) <= range;
            case Targeting.Enemy:
                if (ctx.target == null) return false;
                return Vector2.Distance(ctx.castPos, (Vector2)ctx.target.transform.position) <= range;
            default:
                return true;
        }
    }

    public abstract void Execute(SkillContext ctx);

    protected float TierMul() => tier switch
    {
        Tier.Low => tierMulLow,
        Tier.Mid => tierMulMid,
        _ => tierMulHigh
    };
}

public class SkillContext
{
    public Transform caster;
    public Vector2 castPos;
    public Vector2 aimDir;
    public Vector2 aimPoint;
    public GameObject target;

    public int casterLevel;

    public System.Func<string, bool> IsOffCooldown;
    public System.Action<string, float> StartCooldown;

    public System.Func<string, GameObject> SpawnFromPool;
    public System.Action<string, Vector2> PlayFXAt;
}
