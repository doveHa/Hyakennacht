using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Trap/결초보은")]
public class SK_KyulChoBoeun : SkillBase
{
    [Header("Trap Pool Key")]
    public string trapKey = "KyulTrap";

    [Header("Placement")]
    public float offset = 1.0f;          // 캐스터 앞 배치
    public float feetYOffset = -0.2f;    // 발 높이 보정
    public bool alignToFacing = true;    // 좌우 회전 적용

    [Header("Limits")]
    public int maxActivePerCaster = 5;   // 동시에 최대 개수
    public float trapLifetime = 20f;     // 안 밟으면 자동 회수

    [Header("Effect")]
    public int baseDamage = 20;
    public float stunSeconds = 1.5f;     // Stunnable로 적용되는 시간
    public LayerMask enemyMask;
    public string[] targetTags = new[] { "Enemy" };
    public string hitFxKey;              // 덫이 잡을 때 FX

    // 캐스터별 현재 설치 목록(오래된 것부터 제거)
    static readonly Dictionary<int, LinkedList<PooledObject>> _activeByCaster = new();

    public override void Execute(SkillContext ctx)
    {
        // 쿨타임
        ctx.StartCooldown?.Invoke(skillId, cooldown);

        // 스폰 위치
        float face = Mathf.Sign(ctx.caster.localScale.x);
        Vector2 forward = new Vector2(face, 0f);
        Vector2 origin = ctx.castPos + new Vector2(0f, feetYOffset);
        Vector2 spawnPos = origin + forward * Mathf.Max(0.05f, offset);

        // 풀 생성
        var go = ctx.SpawnFromPool?.Invoke(trapKey);
        if (!go) return;

        go.transform.position = spawnPos;
        if (alignToFacing) go.transform.right = forward;

        var trap = go.GetComponent<TrapPlant2D>();
        if (!trap)
        {
            Debug.LogWarning("[KyulChoBoeun] TrapPlant2D missing on prefab.");
            return;
        }

        // 캐스터별 큐 준비
        int casterKey = ctx.caster.GetInstanceID();
        if (!_activeByCaster.TryGetValue(casterKey, out var list))
        {
            list = new LinkedList<PooledObject>();
            _activeByCaster[casterKey] = list;
        }

        // PooledObject 토큰 확보
        var token = go.GetComponent<PooledObject>();
        if (!token) token = go.AddComponent<PooledObject>(); // 없으면 부착
        token.key = trapKey;  // (선택) 키 동기화

        // 초과 시 가장 오래된 덫 회수
        int cap = Mathf.Max(1, maxActivePerCaster);
        if (list.Count >= cap)
        {
            var oldest = list.First.Value;
            list.RemoveFirst();
            if (oldest) oldest.ReturnToPool();
        }

        // 새 덫 등록
        list.AddLast(token);

        // 스케일링
        int dmg = Mathf.RoundToInt(baseDamage * powerByLevel.Evaluate(ctx.casterLevel) * TierMul());

        // 무장
        trap.Arm(
            damage: dmg,
            stunSeconds: stunSeconds,
            enemyMask: enemyMask,
            targetTags: targetTags,
            hitFxKey: hitFxKey,
            playFxAt: ctx.PlayFXAt,
            onFinish: (t) =>
            {
                // 회수 시 목록에서 제거(안전 탐색)
                var node = list.First;
                while (node != null)
                {
                    var next = node.Next;
                    if (node.Value == token) { list.Remove(node); break; }
                    node = next;
                }
            },
            autoDespawnSeconds: trapLifetime
        );
    }
}
