using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Throw/독 사과")]
public class SK_PoisonApple : SkillBase
{
    [Header("Pool & Prefab")]
    public string projectileKey = "PoisonApple"; 

    [Header("Throw Params")]
    public float speed = 10f;
    public float arcAngleDeg = 35f;      
    public float gravityScale = 2.5f;
    public float life = 4f;

    [Header("Hit Filter")]
    public LayerMask hitMask;           

    [Header("Payload (독 사과)")]
    public float doomSeconds = 3f;       
    public string bubbleFxKey;           

    [Header("FX Trail (선택)")]
    public string trailFxKey;

    public override void Execute(SkillContext ctx)
    {
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        Vector2 origin = ctx.castPos;

        float face = Mathf.Sign(ctx.caster.localScale.x);
        Vector2 dir = new Vector2(face, 0f);   

        var go = ctx.SpawnFromPool?.Invoke(projectileKey);
        if (!go) return;

        var tp = go.GetComponent<ThrownProjectile2D>();
        if (!tp) { Debug.LogWarning("[PoisonApple] ThrownProjectile2D missing"); return; }

        var payload = go.GetComponent<PoisonApplePayload>() ?? go.AddComponent<PoisonApplePayload>();
        payload.doomSeconds = doomSeconds;
        payload.bubbleFxKey = bubbleFxKey;

        tp.Fire(
            owner: ctx.caster,
            origin: origin,
            dir: dir,               // ← 정면
            speed: speed,
            arcAngleDeg: arcAngleDeg,
            lifeSeconds: life,
            gravityScale: gravityScale,
            hitMask: hitMask,
            trailFxKey: trailFxKey,
            fx: ctx.PlayFXAt
        );
    }
}
