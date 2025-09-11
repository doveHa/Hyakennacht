using System.Collections.Generic;
using UnityEngine;

public class HyakkiController2D : MonoBehaviour
{
    public struct Config
    {
        public System.Func<string, GameObject> poolSpawn;
        public System.Action<string, Vector2> playFxAt;

        public string spiritKey;
        public float totalDuration;

        public int combatCount;
        public bool singleWhenNoEnemy;

        public float footYOffset;
        public float spawnRadius;
        public float idleSlotRadius;
        public float arcStartDeg;
        public float arcEndDeg;

        public float detectRadius;
        public LayerMask enemyMask;
        public string[] targetTags;

        public string[] ignoreNameContains;  

        public float moveSpeed;
        public float turnLerp;
        public float standoffDistance;

        public int damage;
        public float attackCooldown;
        public string hitFxKey;
        public float ignoreSameTargetSeconds;
    }

    Config _c;
    float _endTime;
    readonly List<SpiritMinion2D> _actives = new();

    public void Arm(Config c)
    {
        _c = c;
        _endTime = Time.time + Mathf.Max(0.2f, c.totalDuration);

        int desired = DesiredCount();
        for (int i = 0; i < desired; i++) SpawnOne(i, desired);
    }

    void Update()
    {
        if (Time.time >= _endTime) { KillAll(); Destroy(this); return; }

        bool hasEnemy = HasAnyEnemy();
        int desired = hasEnemy ? _c.combatCount : (_c.singleWhenNoEnemy ? 1 : _c.combatCount);

        if (_actives.Count < desired)
        {
            int add = desired - _actives.Count;
            for (int i = 0; i < add; i++) SpawnOne(_actives.Count + i, desired);
        }
        else if (_actives.Count > desired)
        {
            int remove = _actives.Count - desired;
            for (int i = 0; i < remove; i++)
            {
                var s = _actives[_actives.Count - 1];
                _actives.RemoveAt(_actives.Count - 1);
                if (s) s.ForceReturnToPool();
            }
        }
    }

    int DesiredCount()
        => HasAnyEnemy() ? _c.combatCount : (_c.singleWhenNoEnemy ? 1 : _c.combatCount);

    bool HasAnyEnemy()
    {
        var buf = new Collider2D[16];
        Vector2 pos = (Vector2)transform.position;
        int n = Phys2DCompat.OverlapCircle(pos, _c.detectRadius, buf, _c.enemyMask, includeTriggers: true);
        if (n <= 0) return false;

        for (int i = 0; i < n; i++)
        {
            var co = buf[i];
            if (!co) continue;

            // ★ 이름 기준 무시
            if (ShouldIgnoreByName(co.transform, _c.ignoreNameContains)) continue;

            if (_c.targetTags == null || _c.targetTags.Length == 0) return true;

            for (int t = 0; t < _c.targetTags.Length; t++)
            {
                var tag = _c.targetTags[t];
                if (!string.IsNullOrEmpty(tag) && (co.CompareTag(tag) || co.transform.root.CompareTag(tag)))
                    return true;
            }
        }
        return false;
    }

    static bool ShouldIgnoreByName(Transform t, string[] ignores)
    {
        if (ignores == null || ignores.Length == 0 || !t) return false;
        string self = t.name;
        string root = t.root ? t.root.name : string.Empty;
        for (int i = 0; i < ignores.Length; i++)
        {
            var key = ignores[i];
            if (string.IsNullOrEmpty(key)) continue;
            if (self.Contains(key) || root.Contains(key)) return true;
        }
        return false;
    }

    void SpawnOne(int slotIndex, int slotCount)
    {
        var go = _c.poolSpawn?.Invoke(_c.spiritKey);
        if (!go) return;

        float ang0 = _c.arcStartDeg * Mathf.Deg2Rad;
        float ang1 = _c.arcEndDeg * Mathf.Deg2Rad;
        float t = (slotCount <= 1) ? 0.5f : (slotIndex / (float)(slotCount - 1));
        float ang = Mathf.Lerp(ang0, ang1, t);
        Vector2 dir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
        Vector2 feet = (Vector2)transform.position + new Vector2(-1f, _c.footYOffset);
        Vector2 spawnPos = feet + dir * _c.spawnRadius;

        go.transform.position = spawnPos;
        go.transform.right = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector2.right;

        var sp = go.GetComponent<SpiritMinion2D>();
        if (!sp) { Debug.LogWarning("[HyakkiController] SpiritMinion2D missing on prefab."); return; }

        sp.Arm(new SpiritMinion2D.Config
        {
            owner = this.transform,
            life = _endTime - Time.time,
            moveSpeed = _c.moveSpeed,
            turnLerp = _c.turnLerp,
            detectRadius = _c.detectRadius,
            enemyMask = _c.enemyMask,
            targetTags = _c.targetTags,

            damage = _c.damage,
            attackCooldown = _c.attackCooldown,
            hitFxKey = _c.hitFxKey,
            ignoreSameTargetSeconds = _c.ignoreSameTargetSeconds,

            recoilDist = 0f,
            recoilTime = 0.1f,
            enemyKnockbackForce = 0f,
            maxEnemyKnockbackSpeed = 0f,

            orbitRadius = 0f,
            orbitTightness = 0f,
            orbitClockwise = (slotIndex % 2 == 0),

            footYOffset = _c.footYOffset,
            idleSlotRadius = _c.idleSlotRadius,
            slotIndex = slotIndex,
            slotCount = slotCount,

            standoffDistance = _c.standoffDistance,

            despawnOnHit = true,
            onReturned = OnSpiritReturned,
            poolSpawn = _c.poolSpawn,

            ignoreNameContains = _c.ignoreNameContains  
        });

        _actives.Add(sp);
    }

    void OnSpiritReturned(SpiritMinion2D s)
    {
        _actives.Remove(s);
        if (Time.time < _endTime)
        {
            int desired = DesiredCount();
            if (_actives.Count < desired)
                SpawnOne(_actives.Count, desired);
        }
    }

    void KillAll()
    {
        for (int i = 0; i < _actives.Count; i++)
            if (_actives[i]) _actives[i].ForceReturnToPool();
        _actives.Clear();
    }
}
