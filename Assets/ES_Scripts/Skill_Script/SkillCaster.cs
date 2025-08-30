using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class SkillCaster : MonoBehaviour
{
    public SkillBase[] slots;

    // ★ 실제로 움직이고 뒤집히는 주체(Body)와 발사 위치
    public Transform actor;                 // Body
    public FacingProvider2D facing;         // Body에 붙인 FacingProvider2D
    public Transform firePoint;             // Body/FirePos

    public float gcd = 0.2f;
    private readonly Dictionary<string, float> cdEnd = new();
    private float gcdEnd;

    public ObjectPool pool;
    public FXRouter fx;

    void Awake()
    {
        if (!pool) pool = FindFirstObjectByType<ObjectPool>();
        if (!fx) fx = FindFirstObjectByType<FXRouter>();

        // 자동 탐색(비워져 있으면)
        if (!actor)
        {
            var gmPlayer = (GameManager.Manager && GameManager.Manager.Player)
                           ? GameManager.Manager.Player
                           : GameObject.FindGameObjectWithTag("Player"); // 안전망

            Transform root = gmPlayer ? gmPlayer.transform : transform;

            var body = root.Find("Body");
            actor = body ? body : root.GetComponentInChildren<Transform>(true);
        }
        if (actor && !facing) facing = actor.GetComponent<FacingProvider2D>();

        if (actor && !firePoint)
        {
            firePoint = actor.Find("FirePos");
            if (!firePoint) firePoint = actor; // 안전망
        }
    }

    void Update()
    {
        if (slots == null) return;

        if (Input.GetKeyDown(KeyCode.X)) TryCast(slots.Length > 0 ? slots[0] : null);
        if (Input.GetKeyDown(KeyCode.C)) TryCast(slots.Length > 1 ? slots[1] : null);
        if (Input.GetKeyDown(KeyCode.V)) TryCast(slots.Length > 2 ? slots[2] : null);
    }

    void TryCast(SkillBase skill)
    {
        if (skill == null) return;
        if (skill.useGCD && Time.time < gcdEnd) return;

        Vector2 origin = firePoint ? (Vector2)firePoint.position
                                   : (Vector2)(actor ? actor.position : transform.position);

        float face = facing ? facing.FaceSign()
                            : Mathf.Sign((actor ? actor.localScale.x : transform.localScale.x));
        Vector2 forward = new Vector2(face, 0f);

        Vector2 mouseWorld = Camera.main ? (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)
                                         : origin + forward;
        Vector2 aimDir = (mouseWorld - origin).sqrMagnitude > 0.001f ? (mouseWorld - origin).normalized
                                                                      : forward;

        var ctx = new SkillContext
        {
            caster = actor ? actor : transform,  
            castPos = origin,
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
