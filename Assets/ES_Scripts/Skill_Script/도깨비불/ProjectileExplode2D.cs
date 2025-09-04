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
        col.isTrigger = true;        // Ʈ���ŷ� �浹 �Ǵ�
        rb.gravityScale = 0f;        // ����ü�� �߷� X
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

        rb.linearVelocity = dir.normalized * speed;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (Time.time >= _lifeEnd) Explode(null);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // �ڽ�/��ȯ�ڿ��� �浹 ����
        if (_owner && other.transform == _owner) return;

        // ���̾� ����
        if (((1 << other.gameObject.layer) & _hitMask) == 0) return;

        Explode(other);
    }

    void Explode(Collider2D hitCol)
    {
        // ���� ��ġ = ���� ��ġ(�Ǵ� �浹 ����)
        Vector2 pos = transform.position;

        // ���� ����
        var cols = Physics2D.OverlapCircleAll(pos, _explodeRadius, _hitMask);
        for (int i = 0; i < cols.Length; i++)
        {
            // Enemy�� �θ� ���� ���� �����Ƿ� InParent��
            var enemy = cols[i].GetComponentInParent<EnemyStats>();
            if (enemy != null)
                enemy.TakeDamage(_damage);
        }

        // ���� FX (������)
        if (!string.IsNullOrEmpty(_impactFxKey))
        {
            // Ǯ/FXRouter�� �� ���� ��ġ��, ���� ������ ��ü���� ��ƼŬ ����ϵ��� �����ϰų�
            // ������ �ӽ÷� ���� ���� ��(����ȭ�� ������Ʈ�� ����)
            // Example: Instantiate(impactPrefab, pos, Quaternion.identity);
        }

        ReturnToPool();
    }

    void ReturnToPool()
    {
        rb.linearVelocity = Vector2.zero;

        // Ǯ �ݳ�
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
