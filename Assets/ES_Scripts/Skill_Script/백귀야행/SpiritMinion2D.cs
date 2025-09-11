using Manager;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class SpiritMinion2D : MonoBehaviour
{
    public struct Config
    {
        public Transform owner;
        public float life;
        public float moveSpeed;
        public float turnLerp;
        public float detectRadius;
        public LayerMask enemyMask;
        public string[] targetTags;

        public int damage;
        public float attackCooldown;
        public string hitFxKey;
        public float ignoreSameTargetSeconds;

        public float recoilDist, recoilTime, enemyKnockbackForce, maxEnemyKnockbackSpeed;

        public float orbitRadius, orbitTightness;
        public bool orbitClockwise;
        public float footYOffset, idleSlotRadius;
        public int slotIndex, slotCount;

        public float standoffDistance;

        public bool despawnOnHit;
        public System.Action<SpiritMinion2D> onReturned;
        public System.Func<string, GameObject> poolSpawn;

        public string[] ignoreNameContains;

        // 왼쪽일 때 상하 반전이 필요하면 true
        public bool mirrorYWhenLeft;
    }

    [Header("Optional visual root (비워도 됨)")]
    [SerializeField] Transform visualRoot;

    Transform _owner;
    float _lifeEnd;

    float _speed, _turnLerp, _detectRadius;
    LayerMask _enemyMask;
    string[] _targetTags;

    int _damage; float _atkCD;
    float _footYOffset, _idleSlotRadius; int _slotIndex, _slotCount;
    float _standoffDistance; int _orbitDir;
    string _hitFxKey; System.Action<string, Vector2> _playFxAt;
    System.Action<SpiritMinion2D> _onReturned;
    bool _despawnOnHit;
    string[] _ignoreNameContains;
    bool _mirrorYWhenLeft = true;

    Rigidbody2D _rb; Collider2D _col; Animator _anim;

    float _nextAttackTime;
    int _lastHitTargetId = -1; float _ignoreUntil = 0f; float _ignoreSameTargetSeconds = 0.3f;

    Character.Dash _playerDash;
    SpriteRenderer _playerSR;

    SpriteRenderer[] _allSRs;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _anim = GetComponentInChildren<Animator>(true);

        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _col.isTrigger = true;

        _allSRs = GetComponentsInChildren<SpriteRenderer>(true);
        RebindPlayerFacingRefs(); // 처음 한 번 바인딩
    }

    public void Arm(Config c)
    {
        _owner = c.owner;
        _lifeEnd = Time.time + Mathf.Max(0.1f, c.life);

        _speed = Mathf.Max(0f, c.moveSpeed);
        _turnLerp = Mathf.Max(0f, c.turnLerp);
        _detectRadius = Mathf.Max(0f, c.detectRadius);
        _enemyMask = c.enemyMask;
        _targetTags = c.targetTags ?? System.Array.Empty<string>();

        _damage = Mathf.Max(0, c.damage);
        _atkCD = Mathf.Max(0.05f, c.attackCooldown);
        _hitFxKey = c.hitFxKey;
        _ignoreSameTargetSeconds = (c.ignoreSameTargetSeconds > 0f) ? c.ignoreSameTargetSeconds : 0.3f;

        _orbitDir = c.orbitClockwise ? -1 : 1;
        _footYOffset = c.footYOffset;
        _idleSlotRadius = (c.idleSlotRadius > 0f) ? c.idleSlotRadius : 0.8f;
        _slotIndex = Mathf.Max(0, c.slotIndex);
        _slotCount = Mathf.Max(1, c.slotCount);
        _standoffDistance = Mathf.Max(0f, c.standoffDistance);

        _despawnOnHit = c.despawnOnHit;
        _onReturned = c.onReturned;
        _playFxAt = null;

        _ignoreNameContains = c.ignoreNameContains;
        _mirrorYWhenLeft = c.mirrorYWhenLeft;

        _allSRs = GetComponentsInChildren<SpriteRenderer>(true);

        RebindPlayerFacingRefs();

        SyncVisualFacingWithPlayer(force: true);

        _nextAttackTime = Time.time;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (Time.time >= _lifeEnd) { ReturnToPool(); return; }

        if ((_playerDash == null && _playerSR == null) || GameManager.Manager == null || GameManager.Manager.Player == null)
            RebindPlayerFacingRefs();

        var target = AcquireTarget(transform.position, _detectRadius);
        if (target)
        {
            int id = target.GetInstanceID();
            if (Time.time < _ignoreUntil && id == _lastHitTargetId) target = null;
        }

        Vector2 vel = Vector2.zero;

        if (target)
        {
            Vector2 to = (Vector2)target.position - (Vector2)transform.position;
            float dist = to.magnitude;
            Vector2 desiredDir = (_standoffDistance > 0f && dist <= _standoffDistance)
                ? new Vector2(-to.y, to.x).normalized * _orbitDir
                : (to.sqrMagnitude > 1e-6f ? to.normalized : Vector2.zero);

            vel = Vector2.Lerp(_rb.linearVelocity, desiredDir * _speed, Time.deltaTime * _turnLerp);
        }
        else if (_owner)
        {
            Vector2 anchor = (Vector2)_owner.position + new Vector2(0f, _footYOffset);
            float ang = ((_slotIndex + 0.5f) / _slotCount) * Mathf.PI;
            Vector2 home = anchor + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * _idleSlotRadius;

            Vector2 toHome = home - (Vector2)transform.position;
            if (toHome.sqrMagnitude >= 1e-6f)
            {
                Vector2 desired = toHome.normalized * (_speed * 0.6f);
                vel = Vector2.Lerp(_rb.linearVelocity, desired, Time.deltaTime * (_turnLerp * 0.5f));
            }
        }

        _rb.linearVelocity = vel;

        SyncVisualFacingWithPlayer();
    }

    void RebindPlayerFacingRefs()
    {
        var player = GameManager.Manager ? GameManager.Manager.Player : null;
        if (!player) { _playerDash = null; _playerSR = null; return; }

        _playerDash = player.GetComponentInChildren<Character.Dash>(true);

        var body = FindDeepChildByName(player.transform, "Body");
        if (body)
            _playerSR = body.GetComponentInChildren<SpriteRenderer>(true);
        else
            _playerSR = player.GetComponentInChildren<SpriteRenderer>(true);
    }

    void SyncVisualFacingWithPlayer(bool force = false)
    {
        if (_allSRs == null || _allSRs.Length == 0) return;

        bool got, isLeft;
        (got, isLeft) = TryGetPlayerFacingLeft();

        if (!got) return;

        bool flipX = isLeft;
        bool flipY = _mirrorYWhenLeft ? isLeft : false;

        for (int i = 0; i < _allSRs.Length; i++)
        {
            var sr = _allSRs[i];
            if (!sr) continue;
            sr.flipX = flipX;
            sr.flipY = flipY;
        }

        if (visualRoot) visualRoot.localRotation = Quaternion.identity;
    }

    (bool ok, bool isLeft) TryGetPlayerFacingLeft()
    {
        var player = GameManager.Manager ? GameManager.Manager.Player : null;
        if (!player) return (false, false);

        if (_playerDash != null) return (true, _playerDash.IsLeftSight);

        if (_playerSR == null) RebindPlayerFacingRefs();
        if (_playerSR != null)
        {
            bool isLeft = _playerSR.flipX; 
                                           
            return (true, isLeft);
        }

        return (true, player.transform.localScale.x < 0f);
    }

    static Transform FindDeepChildByName(Transform root, string name)
    {
        if (!root) return null;
        if (root.name == name) return root;
        for (int i = 0; i < root.childCount; i++)
        {
            var t = FindDeepChildByName(root.GetChild(i), name);
            if (t) return t;
        }
        return null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & _enemyMask.value) == 0) return;
        if (ShouldIgnoreByName(other.transform)) return;

        var enemy = other.GetComponentInParent<AEnemyStats>();
        if (!enemy) return;
        if (Time.time < _nextAttackTime) return;
        _nextAttackTime = Time.time + _atkCD;

        if (_damage > 0) enemy.TakeDamage(_damage);
        if (_anim) _anim.SetTrigger("Attack");

        _lastHitTargetId = enemy.GetInstanceID();
        _ignoreUntil = Time.time + _ignoreSameTargetSeconds;

        if (_despawnOnHit) ReturnToPool();
    }

    Transform AcquireTarget(Vector2 origin, float radius)
    {
        Collider2D[] buf = new Collider2D[32];
        int n = Phys2DCompat.OverlapCircle(origin, radius, buf, _enemyMask, includeTriggers: true);
        Transform best = null; float bestSqr = float.MaxValue;

        for (int i = 0; i < n; i++)
        {
            var col = buf[i];
            if (!col) continue;
            if (ShouldIgnoreByName(col.transform)) continue;

            if (_targetTags != null && _targetTags.Length > 0)
            {
                bool ok = false;
                for (int j = 0; j < _targetTags.Length; j++)
                {
                    var t = _targetTags[j];
                    if (!string.IsNullOrEmpty(t) && (col.CompareTag(t) || col.transform.root.CompareTag(t)))
                    { ok = true; break; }
                }
                if (!ok) continue;
            }

            float sq = ((Vector2)col.bounds.center - origin).sqrMagnitude;
            if (sq < bestSqr) { bestSqr = sq; best = col.transform.root; }
        }
        return best;
    }

    bool ShouldIgnoreByName(Transform t)
    {
        if (_ignoreNameContains == null || _ignoreNameContains.Length == 0 || !t) return false;
        string self = t.name;
        string root = t.root ? t.root.name : string.Empty;
        for (int i = 0; i < _ignoreNameContains.Length; i++)
        {
            var key = _ignoreNameContains[i];
            if (string.IsNullOrEmpty(key)) continue;
            if (self.Contains(key) || root.Contains(key)) return true;
        }
        return false;
    }

    public void ForceReturnToPool() => ReturnToPool();

    void ReturnToPool()
    {
        _rb.linearVelocity = Vector2.zero;
        var token = GetComponent<PooledObject>();
        if (token != null) token.ReturnToPool();
        else gameObject.SetActive(false);

        _onReturned?.Invoke(this);
    }
}
