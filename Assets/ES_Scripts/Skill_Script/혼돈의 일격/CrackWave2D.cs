using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CrackWave2D : MonoBehaviour
{
    Transform _owner;
    Vector2 _origin;
    Vector2 _forward;
    float _maxLength, _width, _duration;
    int _primaryDamage, _sweepDamage;
    float _stunSec;
    bool _stunPrimaryOnly;
    LayerMask _enemyMask;
    string[] _targetTags;
    string _hitFxKey;
    System.Action<string, Vector2> _playFxAt;

    readonly Collider2D[] _buf = new Collider2D[64];
    readonly HashSet<Enemy_ES> _hitOnce = new();
    Enemy_ES _primaryTarget;
    float _t0;
    bool _armed;

    public void Arm(
        Transform owner, Vector2 origin, Vector2 forward,
        float maxLength, float width, float duration,
        int primaryDamage, int sweepDamage, float stunSeconds, bool stunPrimaryOnly,
        LayerMask enemyMask, string[] targetTags,
        string hitFxKey, System.Action<string, Vector2> fx)
    {
        _owner = owner;
        _origin = origin;
        _forward = forward.normalized;
        _maxLength = Mathf.Max(0.1f, maxLength);
        _width = Mathf.Max(0.1f, width);
        _duration = Mathf.Max(0.05f, duration);
        _primaryDamage = Mathf.Max(0, primaryDamage);
        _sweepDamage = Mathf.Max(0, sweepDamage);
        _stunSec = Mathf.Max(0f, stunSeconds);
        _stunPrimaryOnly = stunPrimaryOnly;
        _enemyMask = enemyMask;
        _targetTags = targetTags ?? System.Array.Empty<string>();
        _hitFxKey = hitFxKey;
        _playFxAt = fx;

        _hitOnce.Clear();
        _primaryTarget = FindPrimaryTarget();
        _t0 = Time.time;
        _armed = true;

        StopAllCoroutines();
        StartCoroutine(CoSweep());
        gameObject.SetActive(true);
    }

    Enemy_ES FindPrimaryTarget()
    {
        // 아주 얇은 박스로 "가장 가까운 전방" 탐색
        float probeLen = _maxLength;
        Vector2 center = _origin + _forward * (probeLen * 0.5f);
        Vector2 size = new Vector2(probeLen, _width * 0.6f);
        float angle = Vector2.SignedAngle(Vector2.right, _forward);

        int n = Physics2D.OverlapBoxNonAlloc(center, size, angle, _buf, _enemyMask);
        float best = float.MaxValue;
        Enemy_ES bestE = null;
        for (int i = 0; i < n; i++)
        {
            var col = _buf[i];
            if (!col) continue;
            if (!TagOk(col.transform.root.gameObject)) continue;

            var e = col.GetComponentInParent<Enemy_ES>();
            if (!e) continue;

            float d = Vector2.Dot((Vector2)e.transform.position - _origin, _forward);
            if (d < 0f) continue; // 뒤는 제외
            if (d < best) { best = d; bestE = e; }
        }
        return bestE;
    }

    IEnumerator CoSweep()
    {
        while (_armed)
        {
            float u = Mathf.Clamp01((Time.time - _t0) / _duration);
            float len = _maxLength * u;

            // 현재 길이에 해당하는 박스 영역
            Vector2 center = _origin + _forward * (len * 0.5f);
            Vector2 size = new Vector2(len, _width);
            float angle = Vector2.SignedAngle(Vector2.right, _forward);

            int n = Physics2D.OverlapBoxNonAlloc(center, size, angle, _buf, _enemyMask);
            for (int i = 0; i < n; i++)
            {
                var col = _buf[i];
                if (!col) continue;
                if (!TagOk(col.transform.root.gameObject)) continue;

                var enemy = col.GetComponentInParent<Enemy_ES>();
                if (!enemy) continue;
                if (_hitOnce.Contains(enemy)) continue; // 1회만

                // 타격 처리
                int dmg = (enemy == _primaryTarget && _primaryDamage > 0) ? _primaryDamage : _sweepDamage;
                if (dmg > 0) enemy.TakeDamage(dmg);

                // 스턴
                if (_stunSec > 0f && (enemy == _primaryTarget || !_stunPrimaryOnly))
                {
                    var st = enemy.GetComponent<Stunnable>();
                    if (st) st.ApplyStun(_stunSec);
                }

                // 피격 FX
                if (!string.IsNullOrEmpty(_hitFxKey))
                    _playFxAt?.Invoke(_hitFxKey, col.bounds.center);

                _hitOnce.Add(enemy);
            }

            if (u >= 1f) break;
            yield return null;
        }

        Finish();
    }

    bool TagOk(GameObject go)
    {
        if (_targetTags == null || _targetTags.Length == 0) return true;
        bool ok = false;
        for (int i = 0; i < _targetTags.Length; i++)
        {
            var t = _targetTags[i];
            if (!string.IsNullOrEmpty(t) && (go.CompareTag(t) || go.transform.root.CompareTag(t))) { ok = true; break; }
        }
        return ok;
    }

    void Finish()
    {
        _armed = false;
        StopAllCoroutines();

        // 풀 반납
        var token = GetComponent<PooledObject>();
        if (token != null) token.ReturnToPool();
        else gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!_armed) return;
        float u = Mathf.Clamp01((Time.time - _t0) / Mathf.Max(0.01f, _duration));
        float len = _maxLength * u;
        Vector2 center = _origin + _forward * (len * 0.5f);
        Vector3 c3 = new Vector3(center.x, center.y, 0);
        Vector3 s3 = new Vector3(len, _width, 0.1f);
        Quaternion rot = Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.right, _forward), Vector3.forward);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Matrix4x4 m = Matrix4x4.TRS(c3, rot, Vector3.one);
        Gizmos.matrix = m;
        Gizmos.DrawCube(Vector3.zero, s3);
        Gizmos.matrix = Matrix4x4.identity;
    }
#endif
}
