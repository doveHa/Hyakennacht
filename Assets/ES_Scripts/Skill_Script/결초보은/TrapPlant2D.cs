using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrapPlant2D : MonoBehaviour
{
    [Header("Animator Params (Optional)")]
    public Animator trapAnimator;           
    public string armedBool = "Armed";    
    public string grabTrigger = "Grab";     

    [Header("Enemy Animator Params (Optional)")]
    public string enemyGrabbedBool = "Grabbed"; 

    Collider2D _col;
    PooledObject _pooled;
    bool _armed;

    int _damage;
    float _stunSeconds;
    LayerMask _enemyMask;
    string[] _targetTags;
    string _hitFxKey;
    System.Action<string, Vector2> _playFxAt;
    System.Action<TrapPlant2D> _onFinish;
    float _autoDespawn;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;
        _pooled = GetComponent<PooledObject>();
        if (!trapAnimator) trapAnimator = GetComponentInChildren<Animator>();
    }

    public void Arm(
        int damage, float stunSeconds,
        LayerMask enemyMask, string[] targetTags,
        string hitFxKey, System.Action<string, Vector2> playFxAt,
        System.Action<TrapPlant2D> onFinish,
        float autoDespawnSeconds = 20f
    )
    {
        _damage = damage;
        _stunSeconds = Mathf.Max(0.05f, stunSeconds);
        _enemyMask = enemyMask;
        _targetTags = targetTags ?? System.Array.Empty<string>();
        _hitFxKey = hitFxKey;
        _playFxAt = playFxAt;
        _onFinish = onFinish;
        _autoDespawn = autoDespawnSeconds;

        _armed = true;
        _col.enabled = true;

        if (trapAnimator && !string.IsNullOrEmpty(armedBool))
            trapAnimator.SetBool(armedBool, true);

        StopAllCoroutines();
        if (_autoDespawn > 0f) StartCoroutine(CoAutoDespawn(_autoDespawn));
    }

    IEnumerator CoAutoDespawn(float t)
    {
        yield return new WaitForSeconds(t);
        Despawn();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_armed) return;

        // ���̾�/�±� ����
        if (((1 << other.gameObject.layer) & _enemyMask.value) == 0) return;
        if (_targetTags.Length > 0)
        {
            bool ok = false;
            for (int i = 0; i < _targetTags.Length; i++)
            {
                string tg = _targetTags[i];
                if (!string.IsNullOrEmpty(tg) && (other.CompareTag(tg) || other.transform.root.CompareTag(tg)))
                { ok = true; break; }
            }
            if (!ok) return;
        }

        var enemyRoot = other.GetComponentInParent<Transform>();
        if (!enemyRoot) return;

        var enemy = enemyRoot.GetComponent<EnemyStats>();
        if (enemy && _damage > 0) enemy.TakeDamage(_damage);

        var stun = enemyRoot.GetComponent<Stunnable>();
        if (!stun) stun = enemyRoot.gameObject.AddComponent<Stunnable>();
        stun.ApplyStun(_stunSeconds);

        Animator enemyAnim = enemyRoot.GetComponentInChildren<Animator>();
        if (enemyAnim && !string.IsNullOrEmpty(enemyGrabbedBool))
            enemyAnim.SetBool(enemyGrabbedBool, true);

        // �� �ߵ� ����
        if (trapAnimator)
        {
            if (!string.IsNullOrEmpty(armedBool)) trapAnimator.SetBool(armedBool, false);
            if (!string.IsNullOrEmpty(grabTrigger)) trapAnimator.SetTrigger(grabTrigger);
        }

        if (!string.IsNullOrEmpty(_hitFxKey))
            _playFxAt?.Invoke(_hitFxKey, other.bounds.center);

        _armed = false;
        _col.enabled = false;

        // ���� ������ �� �ִϸ��̼� ���� + �� ȸ��
        StartCoroutine(CoRelease(enemyAnim));
    }

    IEnumerator CoRelease(Animator enemyAnim)
    {
        yield return new WaitForSeconds(_stunSeconds);

        if (enemyAnim && !string.IsNullOrEmpty(enemyGrabbedBool))
            enemyAnim.SetBool(enemyGrabbedBool, false);

        Despawn();
    }

    void Despawn()
    {
        _onFinish?.Invoke(this);
        if (_pooled) _pooled.ReturnToPool();
        else gameObject.SetActive(false);
    }
}
