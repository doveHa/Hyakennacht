using UnityEngine;

public interface IThrownPayload
{
    void OnImpact(Transform owner, Collider2D hit, Vector2 hitPoint);
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ThrownProjectile2D : MonoBehaviour
{
    Rigidbody2D _rb;
    Collider2D _col;

    Transform _owner;
    float _lifeEnd;
    LayerMask _hitMask;
    bool _armed;

    string _trailFxKey;
    System.Action<string, Vector2> _playFxAt;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _rb.gravityScale = 0f;            // �⺻��(��Ÿ�ӿ� ���)
        _rb.freezeRotation = true;
        _col.isTrigger = true;
    }

    public void Fire(
        Transform owner,
        Vector2 origin,
        Vector2 dir,
        float speed,
        float arcAngleDeg,
        float lifeSeconds,
        float gravityScale,
        LayerMask hitMask,
        string trailFxKey,
        System.Action<string, Vector2> fx
    )
    {
        _owner = owner;
        _hitMask = hitMask;
        _lifeEnd = Time.time + Mathf.Max(0.05f, lifeSeconds);
        _playFxAt = fx;
        _trailFxKey = trailFxKey;

        transform.position = origin;

        var dir3 = (Vector3)dir.normalized;
        var v0 = (Quaternion.AngleAxis(arcAngleDeg, Vector3.forward) * dir3) * speed;

        _rb.gravityScale = gravityScale;
        _rb.linearVelocity = (Vector2)v0;

        _armed = true;

        if (!string.IsNullOrEmpty(_trailFxKey))
            _playFxAt?.Invoke(_trailFxKey, origin);
    }

    void Update()
    {
        if (!_armed) return;
        if (Time.time >= _lifeEnd) { Despawn(); }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_armed) return;

        if (_owner && (other.transform == _owner || other.transform.IsChildOf(_owner))) return;

        if (((1 << other.gameObject.layer) & _hitMask.value) == 0) return;

        var payloads = GetComponents<IThrownPayload>();
        Vector2 hitPoint = other.bounds.ClosestPoint(transform.position);
        for (int i = 0; i < payloads.Length; i++)
            payloads[i].OnImpact(_owner, other, hitPoint);

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
