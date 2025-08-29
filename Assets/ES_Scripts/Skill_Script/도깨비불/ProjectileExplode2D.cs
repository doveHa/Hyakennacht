using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ProjectileExplode2D : MonoBehaviour
{
    Rigidbody2D rb;
    Transform _owner;
    int _damage;
    float _lifeEnd;
    LayerMask _hitMask;
    float _explodeRadius;
    string _impactFxKey, _trailFxKey;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;        // 트리거로 충돌 판단
        rb.gravityScale = 0f;        // 투사체라 중력 X
    }

    public void Fire(
        Transform owner, int damage, float speed, float life,
        LayerMask hitMask, float explodeRadius, string impactFxKey, string trailFxKey,
        Vector2 dir)  
    {
        _owner = owner;
        _damage = damage;
        _lifeEnd = Time.time + life;
        _hitMask = hitMask;
        _explodeRadius = explodeRadius;
        _impactFxKey = impactFxKey;
        _trailFxKey = trailFxKey;

        rb.velocity = dir.normalized * speed;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (Time.time >= _lifeEnd) Explode(null);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 자신/소환자와의 충돌 무시
        if (_owner && other.transform == _owner) return;

        // 레이어 필터
        if (((1 << other.gameObject.layer) & _hitMask) == 0) return;

        Explode(other);
    }

    void Explode(Collider2D hitCol)
    {
        // 폭발 위치 = 현재 위치(또는 충돌 지점)
        Vector2 pos = transform.position;

        // 피해 적용
        var cols = Physics2D.OverlapCircleAll(pos, _explodeRadius, _hitMask);
        for (int i = 0; i < cols.Length; i++)
        {
            // Enemy가 부모에 있을 수도 있으므로 InParent로
            var enemy = cols[i].GetComponentInParent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(_damage);
        }

        // 폭발 FX (있으면)
        if (!string.IsNullOrEmpty(_impactFxKey))
        {
            // 풀/FXRouter를 못 쓰는 위치니, 폭발 프리팹 자체에서 파티클 재생하도록 구성하거나
            // 간단히 임시로 새로 만들어도 됨(최적화는 프로젝트에 맞춰)
            // Example: Instantiate(impactPrefab, pos, Quaternion.identity);
        }

        ReturnToPool();
    }

    void ReturnToPool()
    {
        rb.velocity = Vector2.zero;

        // 풀 반납
        var token = GetComponent<PooledObject>();
        if (token != null) token.ReturnToPool();
        else gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, _explodeRadius);
    }
#endif
}
