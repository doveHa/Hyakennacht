using Character;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCaster : MonoBehaviour
{
    public SkillBase[] slots;

    [Header("Actor (실제 모델)")]
    public Transform actor;      // Witch/Body
    public Transform firePoint;  // Body/FirePos

    private SpriteRenderer actorSR; // 읽기용
    private Dash actorDash;         // 읽기용

    private Transform facingProxy;

    public float gcd = 0.2f;
    private readonly Dictionary<string, float> cdEnd = new();
    private float gcdEnd;

    public ObjectPool pool;
    public FXRouter fx;

    void Awake()
    {
        if (!pool) pool = FindFirstObjectByType<ObjectPool>();
        if (!fx) fx = FindFirstObjectByType<FXRouter>();

        // Body / FirePos 자동 탐색
        if (!actor)
        {
            var root = (GameManager.Manager && GameManager.Manager.Player)
                       ? GameManager.Manager.Player.transform
                       : transform;
            actor = root.Find("Body") ?? root;
        }
        if (!firePoint)
        {
            firePoint = actor.Find("FirePos");
            if (!firePoint) firePoint = actor;
        }

        actorSR = actor.GetComponentInChildren<SpriteRenderer>(true);
        actorDash = actor.GetComponentInChildren<Dash>(true);

        var proxyGo = new GameObject("FacingProxy");
        facingProxy = proxyGo.transform;
        facingProxy.SetParent(actor, worldPositionStays: false);
        facingProxy.localPosition = Vector3.zero;
        facingProxy.localRotation = Quaternion.identity;
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
        if (!skill) return;
        if (skill.useGCD && Time.time < gcdEnd) return;

        bool isLeft =
            (actorDash != null) ? actorDash.IsLeftSight :
            (actorSR != null) ? actorSR.flipX :
            (actor.localScale.x < 0f);

        float face = isLeft ? -1f : 1f;

        // === 프록시에만 방향 기록 ===
        var ps = facingProxy.localScale;
        facingProxy.localScale = new Vector3((face < 0 ? -1f : 1f), (ps.y == 0 ? 1f : ps.y), (ps.z == 0 ? 1f : ps.z));
        facingProxy.localPosition = Vector3.zero; 

        Vector2 castPos = firePoint ? (Vector2)firePoint.position : (Vector2)actor.position;
        Vector2 forward = new Vector2(face, 0f);
        Vector2 mouseWorld = Camera.main
            ? (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)
            : castPos + forward;
        Vector2 aimDir = (mouseWorld - castPos).sqrMagnitude > 0.0001f ? (mouseWorld - castPos).normalized : forward;

        var ctx = new SkillContext
        {
            caster = facingProxy, 
            castPos = castPos,     
            aimDir = aimDir,
            aimPoint = mouseWorld,
            target = null,
            casterLevel = 1,

            IsOffCooldown = (id) => !cdEnd.ContainsKey(id) || Time.time >= cdEnd[id],
            StartCooldown = (id, cd) => cdEnd[id] = Time.time + cd,

            SpawnFromPool = (key) => pool ? pool.Spawn(key) : null,
            PlayFXAt = (key, pos) => { if (fx) fx.PlayAt(key, pos); }
        };

        if (!skill.CanCast(ctx)) return;

        if (skill.castTime > 0f) StartCoroutine(CastRoutine(skill, ctx));
        else CastNow(skill, ctx);
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