using System.Collections;
using UnityEngine;

public class ClockMarker : MonoBehaviour
{
    Transform _target;
    Vector2 _offset;
    Coroutine _follow;

    public void Arm(Transform target, Vector2 offset, float life)
    {
        _target = target;
        _offset = offset;

        if (_follow != null) StopCoroutine(_follow);
        _follow = StartCoroutine(CoFollow(life));
    }

    IEnumerator CoFollow(float t)
    {
        float end = Time.time + t;
        while (Time.time < end && _target)
        {
            transform.position = (Vector2)_target.position + _offset;
            yield return null;
        }
        ReturnToPool();
    }

    void OnDisable()
    {
        _target = null;
        _follow = null;
    }

    void ReturnToPool()
    {
        var pooled = GetComponent<PooledObject>();
        if (pooled) pooled.ReturnToPool();
        else gameObject.SetActive(false);
    }
}
