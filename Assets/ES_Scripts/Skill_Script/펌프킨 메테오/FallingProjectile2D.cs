using UnityEngine;

public interface IFallPayload
{
    void OnImpact(Transform owner, Vector2 hitPoint, Collider2D hit);
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingProjectile2D : MonoBehaviour
{
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
        _rb.freezeRotation = true;
        _col.isTrigger = true; // �浹�� Ʈ���ŷ� ó��
    }

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

        transform.position = spawnPos;
        transform.right = initialVelocity.sqrMagnitude > 0.0001f ? initialVelocity.normalized : Vector2.down;

        _rb.gravityScale = gravityScale;  // 0�̸� ��� ����
        _rb.linearVelocity = initialVelocity;

        _armed = true;

        if (!string.IsNullOrEmpty(_spawnFx)) _playFxAt?.Invoke(_spawnFx, spawnPos);
        if (!string.IsNullOrEmpty(_trailFx)) _playFxAt?.Invoke(_trailFx, spawnPos);
    }

    void Update()
    {
        if (!_armed) return;
        if (Time.time >= _lifeEnd) Despawn();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_armed) return;

        // ��ȯ��/�ڽ� ����
        if (_owner && (other.transform == _owner || other.transform.IsChildOf(_owner))) return;

        // hitMask==0�̸� ��� ���̾� ���
        if (_hitMask.value != 0 && ((1 << other.gameObject.layer) & _hitMask.value) == 0) return;

        var payloads = GetComponents<IFallPayload>();
        Vector2 hitPoint = other.bounds.ClosestPoint(transform.position);
        for (int i = 0; i < payloads.Length; i++)
            payloads[i].OnImpact(_owner, hitPoint, other);

        if (!string.IsNullOrEmpty(_impactFx)) _playFxAt?.Invoke(_impactFx, transform.position);

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
