using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RiftZone2D : MonoBehaviour
{
    Transform _owner;
    float _duration, _radius, _frontArcDeg;
    int _tickDps; float _tickInterval;
    float _pullForce, _maxPullSpeed;
    LayerMask _mask;
    string _impactFxKey;
    System.Action<string, Vector2> _playFxAt;
    string[] _targetTags;

    readonly Collider2D[] _buf = new Collider2D[64];
    float _endTime;
    bool _armed;

    public void Arm(
        Transform owner,
        float duration,
        float radius,
        float frontArcDeg,
        int tickDamagePerSec,
        float tickInterval,
        float pullForce,
        float maxPullSpeed,
        LayerMask mask,
        string impactFxKey,
        System.Action<string, Vector2> fx,
        string[] targetTags
    )
    {
        _owner = owner;
        _duration = duration;
        _radius = radius;
        _frontArcDeg = Mathf.Clamp(frontArcDeg, 0f, 180f);
        _tickDps = Mathf.Max(0, tickDamagePerSec);
        _tickInterval = Mathf.Max(0.05f, tickInterval);
        _pullForce = pullForce;
        _maxPullSpeed = maxPullSpeed;
        _mask = mask;
        _impactFxKey = impactFxKey;
        _playFxAt = fx;
        _targetTags = targetTags ?? System.Array.Empty<string>();

        _endTime = Time.time + _duration;
        _armed = true;

        StopAllCoroutines();
        StartCoroutine(CoDamageTicks());
        gameObject.SetActive(true);
    }

    void OnDisable()
    {
        _armed = false;
        StopAllCoroutines();
    }

    void FixedUpdate()
    {
        if (!_armed) return;

        Vector2 center = transform.position;
        int n = Phys2DCompat.OverlapCircle(center, _radius, _buf, _mask, includeTriggers: true);

        if (n <= 0) { if (Time.time >= _endTime) Finish(); return; }

        Vector2 fwd = (Vector2)transform.right;
        float halfArc = _frontArcDeg * 0.5f;

        const float innerStopRadius = 0.25f;

        for (int i = 0; i < n; i++)
        {
            var col = _buf[i];
            if (!col) continue;
            if (!PassesFilter(col)) continue; // �� �ñ״�ó �ٲ� (�Ʒ� ����)

            Vector2 pos = col.bounds.center;
            Vector2 to = center - pos;
            float dist = to.magnitude;
            if (dist < Mathf.Epsilon) continue;

            // ���� ��ä�ø�
            if (Vector2.Angle(fwd, -to) > halfArc) continue;

            var rb = col.attachedRigidbody;
            if (rb != null && rb.bodyType != RigidbodyType2D.Kinematic)
            {
                Vector2 dir = to / dist;
                float arriveRadius = _radius * 0.7f;                  // ���� ���� ����
                float speedScale = Mathf.Clamp01(dist / arriveRadius); // 0~1
                Vector2 desiredVel = dir * (_maxPullSpeed * speedScale);

                Vector2 steering = desiredVel - rb.linearVelocity;
                // �� ����(�ʹ� ū ���� ����)
                float maxAccel = _pullForce; // ForceMode2D.Force ����
                if (steering.magnitude > maxAccel)
                    steering = steering.normalized * maxAccel;

                rb.AddForce(steering, ForceMode2D.Force);

                if (dist <= innerStopRadius)
                {
                    rb.linearVelocity = Vector2.zero;
                    // rb.MovePosition(center);
                }
                else
                {
                    float awayDot = Vector2.Dot(rb.linearVelocity, -dir);
                    if (awayDot > 0f)
                        rb.linearVelocity -= (-dir) * (awayDot * 0.3f);
                }

                if (rb.linearVelocity.magnitude > _maxPullSpeed)
                    rb.linearVelocity = rb.linearVelocity.normalized * _maxPullSpeed;
            }
        }

        if (Time.time >= _endTime) Finish();
    }

    IEnumerator CoDamageTicks()
    {
        float dmgPerTickF = _tickDps * _tickInterval;
        int baseTick = Mathf.FloorToInt(dmgPerTickF);
        float carry = dmgPerTickF - baseTick;

        while (_armed && Time.time < _endTime)
        {
            Vector2 center = transform.position;
            Vector2 fwd = transform.right;
            float halfArc = _frontArcDeg * 0.5f;

            int n = Phys2DCompat.OverlapCircle(center, _radius, _buf, _mask, includeTriggers: true);

            for (int i = 0; i < n; i++)
            {
                var col = _buf[i];
                if (!col) continue;
                if (!PassesFilter(col)) continue; // �� ����

                Vector2 to = center - (Vector2)col.bounds.center;
                if (Vector2.Angle(fwd, -to) > halfArc) continue;

                int tick = baseTick;
                if (carry > 0f && Random.value < carry) tick++;

                if (tick > 0)
                {
                    // �� �θ���� ã�Ƽ� Enemy ����
                    var enemy = col.GetComponentInParent<Enemy_ES>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(tick);

                        if (!string.IsNullOrEmpty(_impactFxKey))
                            _playFxAt?.Invoke(_impactFxKey, col.transform.position);
                    }
                }
            }

            yield return new WaitForSeconds(_tickInterval);
        }

        Finish();
    }

    bool PassesFilter(Collider2D col)
    {
        var go = col.gameObject;

        // �±� üũ(��Ʈ/�θ���� ���)
        if (_targetTags != null && _targetTags.Length > 0)
        {
            bool tagOk = false;

            // ���� ������Ʈ �±�
            for (int i = 0; i < _targetTags.Length; i++)
            {
                var t = _targetTags[i];
                if (!string.IsNullOrEmpty(t) && go.CompareTag(t)) { tagOk = true; break; }
            }

            // �θ�(��Ʈ) �±�
            if (!tagOk)
            {
                var root = go.transform.root.gameObject;
                for (int i = 0; i < _targetTags.Length; i++)
                {
                    var t = _targetTags[i];
                    if (!string.IsNullOrEmpty(t) && root.CompareTag(t)) { tagOk = true; break; }
                }
            }

            if (!tagOk) return false;
        }

        // Enemy ������Ʈ(�θ����)
        return col.GetComponentInParent<Enemy_ES>() != null;
    }

    void Finish()
    {
        if (!_armed) return;
        _armed = false;
        StopAllCoroutines();

        var token = GetComponent<PooledObject>();
        if (token != null) token.ReturnToPool();
        else gameObject.SetActive(false); // (���)
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0.6f, 1f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, _radius);

        Vector3 c = transform.position;
        Vector3 fwd = transform.right;
        float half = _frontArcDeg * 0.5f;
        Gizmos.color = new Color(0, 0.6f, 1f, 0.35f);
        Gizmos.DrawLine(c, c + Quaternion.AngleAxis( half, Vector3.forward) * fwd * _radius);
        Gizmos.DrawLine(c, c + Quaternion.AngleAxis(-half, Vector3.forward) * fwd * _radius);
    }
#endif
}
