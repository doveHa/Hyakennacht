using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FirePatch2D : MonoBehaviour
{
    float _duration, _radius, _tickInterval;
    int _dps;
    LayerMask _enemyMask;
    string[] _tags;

    System.Action<string, Vector2> _fx;
    string _tickFx;

    bool _armed;
    readonly Collider2D[] _buf = new Collider2D[64];

    public void Arm(
        Vector2 center,
        float radius,
        float duration,
        int damagePerSecond,
        float tickInterval,
        LayerMask enemyMask,
        string[] targetTags,
        string tickFxKey,
        System.Action<string, Vector2> fx
    )
    {
        transform.position = center;
        _radius = Mathf.Max(0.1f, radius);
        _duration = Mathf.Max(0.1f, duration);
        _dps = Mathf.Max(0, damagePerSecond);
        _tickInterval = Mathf.Max(0.05f, tickInterval);
        _enemyMask = enemyMask;
        _tags = targetTags ?? System.Array.Empty<string>();
        _tickFx = tickFxKey;
        _fx = fx;

        _armed = true;
        StopAllCoroutines();
        StartCoroutine(CoTick());
    }

    IEnumerator CoTick()
    {
        float end = Time.time + _duration;
        float dmgPerTickF = _dps * _tickInterval; // √ ¥Á dps °Ê ∆Ω¥Á
        int baseTick = Mathf.FloorToInt(dmgPerTickF);
        float carry = dmgPerTickF - baseTick;

        while (Time.time < end && _armed)
        {
            int n = Physics2D.OverlapCircleNonAlloc(transform.position, _radius, _buf, _enemyMask);
            for (int i = 0; i < n; i++)
            {
                var co = _buf[i];
                if (!co) continue;

                if (_tags.Length > 0)
                {
                    bool ok = false;
                    for (int t = 0; t < _tags.Length; t++)
                    {
                        var tg = _tags[t];
                        if (!string.IsNullOrEmpty(tg) && (co.CompareTag(tg) || co.transform.root.CompareTag(tg)))
                        { ok = true; break; }
                    }
                    if (!ok) continue;
                }

                int tick = baseTick + ((carry > 0f && Random.value < carry) ? 1 : 0);
                if (tick > 0)
                {
                    var enemy = co.GetComponentInParent<Enemy_ES>();
                    if (enemy) enemy.TakeDamage(tick);
                    if (!string.IsNullOrEmpty(_tickFx)) _fx?.Invoke(_tickFx, co.bounds.center);
                }
            }

            yield return new WaitForSeconds(_tickInterval);
        }

        Despawn();
    }

    void Despawn()
    {
        _armed = false;
        var po = GetComponent<PooledObject>();
        if (po) po.ReturnToPool();
        else gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.35f, 0.1f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, _radius > 0 ? _radius : 1.5f);
    }
#endif
}
