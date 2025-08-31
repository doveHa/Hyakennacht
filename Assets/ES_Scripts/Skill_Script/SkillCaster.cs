using Character;
using Character.Skill;
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

        EnsureActorRefs(force: true);
        EnsureFacingProxy();
        StartCoroutine(RebindNextFrame());
    }

    IEnumerator RebindNextFrame()
    {
        yield return null; // 한 프레임 대기 후, Witch/Body가 생성된 뒤 다시 바인딩
        EnsureActorRefs(force: true);
        EnsureFacingProxy();
    }

    void EnsureActorRefs(bool force = false)
    {
        var gmPlayer = (GameManager.Manager && GameManager.Manager.Player)
                       ? GameManager.Manager.Player.transform
                       : transform;

        // Body를 깊이 검색
        if (force || actor == null || actor.name != "Body")
        {
            actor = FindDeepChildByName(gmPlayer, "Body") ?? gmPlayer;  // 실패 시 Player로 폴백
        }

        if (force || firePoint == null || firePoint.name != "FirePos")
        {
            firePoint = FindDeepChildByName(actor, "FirePos") ?? actor;
        }

        actorSR = actor.GetComponentInChildren<SpriteRenderer>(true);
        actorDash = actor.GetComponentInChildren<Dash>(true);
    }

    void EnsureFacingProxy()
    {
        if (!actor) return;

        var exist = actor.Find("FacingProxy");
        if (exist) facingProxy = exist;

        if (!facingProxy)
        {
            var stray = transform.root.Find("FacingProxy");
            facingProxy = stray ? stray : new GameObject("FacingProxy").transform;
        }

        if (facingProxy.parent != actor)
            facingProxy.SetParent(actor, worldPositionStays: false);

        facingProxy.localPosition = Vector3.zero;
        facingProxy.localRotation = Quaternion.identity;
        if (facingProxy.localScale == Vector3.zero) facingProxy.localScale = Vector3.one;
    }

    Transform FindDeepChildByName(Transform root, string name)
    {
        if (!root) return null;
        if (root.name == name) return root;
        for (int i = 0; i < root.childCount; i++)
        {
            var t = FindDeepChildByName(root.GetChild(i), name);
            if (t) return t;
        }
        return null;
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

        EnsureActorRefs(force: (actor == null || actor.name != "Body" || firePoint == null || firePoint.name != "FirePos"));
        EnsureFacingProxy();

        bool isLeft =
            (actorDash != null) ? actorDash.IsLeftSight :
            (actorSR != null) ? actorSR.flipX :
            (actor.localScale.x < 0f);

        float face = isLeft ? -1f : 1f;

        var ps = facingProxy.localScale;
        facingProxy.localScale = new Vector3(face < 0 ? -1f : 1f,
                                             ps.y == 0 ? 1f : ps.y,
                                             ps.z == 0 ? 1f : ps.z);
        facingProxy.localPosition = Vector3.zero;

        Vector2 castPos = firePoint ? (Vector2)firePoint.position : (Vector2)actor.position;
        Vector2 forward = new Vector2(face, 0f);
        Vector2 mouseWorld = Camera.main
            ? (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)
            : castPos + forward;
        Vector2 aimDir = (mouseWorld - castPos).sqrMagnitude > 1e-6f ? (mouseWorld - castPos).normalized : forward;

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

    public void RegisterSkill(int slotIndex, SkillBase skillName)
    {
        if (slots == null || slotIndex < 0 || slotIndex >= slots.Length)
        {
            Debug.LogWarning("유효하지 않은 슬롯 인덱스");
            return;
        }

        string path = "Skills/" + skillName;
        SkillBase newSkill = Resources.Load<SkillBase>(path);

        if (newSkill == null)
        {
            Debug.LogWarning($"스킬 '{skillName}'을 Resources 폴더에서 찾을 수 없습니다.");
            return;
        }

        if (slots[slotIndex] != null)
        {
            slots[slotIndex] = null;
        }

        slots[slotIndex] = newSkill;
        Debug.Log($"슬롯 {slotIndex}번에 스킬 {newSkill.name} 등록");
    }

}