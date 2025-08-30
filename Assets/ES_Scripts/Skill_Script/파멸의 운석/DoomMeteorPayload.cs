using UnityEngine;

/// 파멸의 운석 전용 페이로드: 적에 닿으면 즉시 피해, 끝.
public class DoomMeteorPayload : MonoBehaviour, IFallPayload
{
    [Header("Impact Damage")]
    public int impactDamage = 80;   // SO에서 최종 주입

    public void OnImpact(Transform owner, Vector2 hitPoint, Collider2D hit)
    {
        var enemy = hit.GetComponentInParent<Enemy_ES>();
        if (enemy && impactDamage > 0)
            enemy.TakeDamage(impactDamage);
        // 추가 이펙트/넉백 안 함. FallingProjectile2D가 바로 ReturnToPool 처리.
    }
}
