using UnityEngine;

public interface IFallPayload
{
    void OnImpact(Transform owner, Vector2 hitPoint, Collider2D hit);
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingProjectile2D : MonoBehaviour
{
    [Header("Visual")]
    [Tooltip("스프라이트/파티클이 달린 자식 Transform (없으면 생략 가능)")]
    public Transform visualRoot;
    [Tooltip("시각 루트의 회전을 항상 0도로 유지")]
    public bool lockVisualRotation = true;
    [Tooltip("초기 속도의 X부호에 따라 시각만 좌우 반전")]
    public bool flipVisualByX = true;

    Rigidbody2D _rb;
    Collider2D _col;

    Transform _owner;
    float _lifeEnd;
    LayerMask _hitMask;
    bool _armed;

    string _spawnFx, _impactFx, _trailFx;
    System.Action<string, Vector2> _playFxAt;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();

        // 물리 회전/충돌 모드
        _rb.freezeRotation = true;
        _col.isTrigger = true;
    }

    /// <summary>
    /// 낙하체 발사 (프리팹 회전은 유지, transform은 절대 돌리지 않음)
    /// </summary>
    public void Fire(
        Transform owner,
        Vector2 spawnPos,
        Vector2 initialVelocity,
        float gravityScale,
        float lifeSeconds,
        LayerMask hitMask,
        string spawnFxKey,
        string trailFxKey,
        string impactFxKey,
        System.Action<string, Vector2> fx
    )
    {
        _owner = owner;
        _hitMask = hitMask;
        _lifeEnd = Time.time + Mathf.Max(0.2f, lifeSeconds);

        _spawnFx = spawnFxKey;
        _impactFx = impactFxKey;
        _trailFx = trailFxKey;
        _playFxAt = fx;

        // 위치만 배치. 회전은 프리팹 원래 값을 유지
        transform.position = spawnPos;
        // transform.rotation = transform.rotation; // (의미상 유지)

        // 물리값 설정 (회전 금지)
        _rb.gravityScale = gravityScale;
        _rb.linearVelocity = initialVelocity; // Unity 6
        _rb.angularVelocity = 0f;

        _armed = true;

        // VFX
        if (!string.IsNullOrEmpty(_spawnFx)) _playFxAt?.Invoke(_spawnFx, spawnPos);
        if (!string.IsNullOrEmpty(_trailFx)) _playFxAt?.Invoke(_trailFx, spawnPos);

        // 시각 루트 처리 (회전 고정 + 좌우 반전만)
        if (visualRoot)
        {
            if (lockVisualRotation) visualRoot.rotation = Quaternion.identity;

            if (flipVisualByX)
            {
                float sx = Mathf.Sign(initialVelocity.x);
                var ls = visualRoot.localScale;
                // x부호만 반영 (0이면 기존 유지)
                visualRoot.localScale = new Vector3(
                    (Mathf.Approximately(sx, 0f) ? ls.x : Mathf.Abs(ls.x) * (sx < 0 ? -1f : 1f)),
                    ls.y, ls.z
                );
            }
        }

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!_armed) return;

        if (Time.time >= _lifeEnd)
        {
            Despawn();
            return;
        }

        // 외부에서 회전이 바뀌더라도 시각만 고정
        if (lockVisualRotation && visualRoot)
            visualRoot.rotation = Quaternion.identity;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_armed) return;

        // 소유자와의 충돌 무시
        if (_owner && (other.transform == _owner || other.transform.IsChildOf(_owner)))
            return;

        // hitMask가 설정되어 있으면 필터링
        if (_hitMask.value != 0 && ((1 << other.gameObject.layer) & _hitMask.value) == 0)
            return;

        // 페이로드 호출
        var payloads = GetComponents<IFallPayload>();
        Vector2 hitPoint = other.bounds.ClosestPoint(transform.position);
        for (int i = 0; i < payloads.Length; i++)
            payloads[i].OnImpact(_owner, hitPoint, other);

        // 임팩트 FX
        if (!string.IsNullOrEmpty(_impactFx))
            _playFxAt?.Invoke(_impactFx, hitPoint);

        Despawn();
    }

    void Despawn()
    {
        _armed = false;
        _rb.linearVelocity = Vector2.zero;

        var token = GetComponent<PooledObject>();
        if (token) token.ReturnToPool();
        else gameObject.SetActive(false);
    }
}
