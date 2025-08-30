using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCaster : MonoBehaviour
{
    public SkillBase[] slots;
    public Transform firePoint;
    public float gcd = 0.2f;

    private readonly Dictionary<string, float> cdEnd = new();
    private float gcdEnd;

    public ObjectPool pool;
    public FXRouter fx;

    void Awake()
    {
        if (!pool) pool = FindFirstObjectByType<ObjectPool>();
        if (!fx) fx = FindFirstObjectByType<FXRouter>();
        if (!firePoint)
        {
            var body = transform.Find("Body");
            if (body)
            {
                var fp = body.Find("FirePos");
                if (fp) firePoint = fp;
            }
        }
    }

    void Update()
    {
        if (slots == null) return;

        if (Input.GetKeyDown(KeyCode.F)) TryCast(slots.Length > 0 ? slots[0] : null);
        if (Input.GetKeyDown(KeyCode.G)) TryCast(slots.Length > 1 ? slots[1] : null);
        if (Input.GetKeyDown(KeyCode.H)) TryCast(slots.Length > 2 ? slots[2] : null);
    }

    void TryCast(SkillBase skill)
    {
        if (skill == null) return;
        if (skill.useGCD && Time.time < gcdEnd) return;

        Vector2 mouseWorld = Camera.main ? (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)
                                         : (Vector2)transform.position;
        Vector2 castPos = firePoint ? (Vector2)firePoint.position : (Vector2)transform.position;
        Vector2 aimDir = (mouseWorld - castPos).normalized;

        var ctx = new SkillContext
        {
            caster = transform,
            castPos = castPos,
            aimDir = aimDir,
            aimPoint = mouseWorld,
            target = null,
            casterLevel = 1,

            IsOffCooldown = (id) => !cdEnd.ContainsKey(id) || Time.time >= cdEnd[id],
            StartCooldown = (id, cd) => cdEnd[id] = Time.time + cd,

            SpawnFromPool = (key) => pool ? pool.Spawn(key) : null,
            PlayFXAt = (key, pos) => { if (fx != null) fx.PlayAt(key, pos); }
        };

        if (!skill.CanCast(ctx)) return;

        if (skill.castTime > 0f)
            StartCoroutine(CastRoutine(skill, ctx));
        else
            CastNow(skill, ctx);
    }

    IEnumerator CastRoutine(SkillBase s, SkillContext ctx)
    {
        yield return new WaitForSeconds(s.castTime);
        CastNow(s, ctx);
    }

    void CastNow(SkillBase s, SkillContext ctx)
    {
        if (s.useGCD) gcdEnd = Time.time + gcd;
        s.Execute(ctx);
    }
}
