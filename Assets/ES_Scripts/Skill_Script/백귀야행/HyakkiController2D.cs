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
    int _spawnSerial = 0;

    public void Arm(Config c)
    {
        _c = c;
        _endTime = Time.time + Mathf.Max(0.2f, c.totalDuration);

        // �ʱ� ����
        int desired = DesiredCount();
        for (int i = 0; i < desired; i++) SpawnOne(i, desired);
    }

    void Update()
    {
        if (Time.time >= _endTime) { KillAll(); Destroy(this); return; }

        // ���� �ֺ��� ���� �ִ��� Ȯ��
        bool hasEnemy = HasAnyEnemy();
        int desired = hasEnemy ? _c.combatCount : (_c.singleWhenNoEnemy ? 1 : _c.combatCount);

        // �����ϸ� ����, ������ ���� ����
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
    {
        return HasAnyEnemy() ? _c.combatCount : (_c.singleWhenNoEnemy ? 1 : _c.combatCount);
    }

    bool HasAnyEnemy()
    {
        var buf = new Collider2D[16];
        Vector2 pos = (Vector2)transform.position;
        int n = Physics2D.OverlapCircleNonAlloc(pos, _c.detectRadius, buf, _c.enemyMask);
        if (n <= 0) return false;
        if (_c.targetTags == null || _c.targetTags.Length == 0) return true;
        for (int i = 0; i < n; i++)
        {
            var co = buf[i];
            if (!co) continue;
            for (int t = 0; t < _c.targetTags.Length; t++)
            {
                var tag = _c.targetTags[t];
                if (!string.IsNullOrEmpty(tag) && (co.CompareTag(tag) || co.transform.root.CompareTag(tag)))
                    return true;
            }
        }
        return false;
    }

    void SpawnOne(int slotIndex, int slotCount)
    {
        var go = _c.poolSpawn?.Invoke(_c.spiritKey);
        if (!go) return;

        // �Ʒ� �ݿ� ���� ��ġ ���(�� ����)
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

        // ȥ�� ����(�ܹ߼�: ���� ������ ��� Ǯ ��ȯ)
        sp.Arm(new SpiritMinion2D.Config
        {
            owner = this.transform,
            life = _endTime - Time.time, // ���� �ð���ŭ ���
            moveSpeed = _c.moveSpeed,
            turnLerp = _c.turnLerp,
            detectRadius = _c.detectRadius,
            enemyMask = _c.enemyMask,
            targetTags = _c.targetTags,

            damage = _c.damage,
            attackCooldown = _c.attackCooldown,
            hitFxKey = _c.hitFxKey,
            ignoreSameTargetSeconds = _c.ignoreSameTargetSeconds,

            // �ܹ߼�: ������/�˹� ��� �� ��
            recoilDist = 0f,
            recoilTime = 0.1f,
            enemyKnockbackForce = 0f,
            maxEnemyKnockbackSpeed = 0f,

            // �˵� OFF �� ��-��Ŀ ���� ���
            orbitRadius = 0f,
            orbitTightness = 0f,
            orbitClockwise = (slotIndex % 2 == 0),

            // ����/��� ����
            footYOffset = _c.footYOffset,
            idleSlotRadius = _c.idleSlotRadius,
            slotIndex = slotIndex,
            slotCount = slotCount,

            // ���� �� �ٹ��� ����
            standoffDistance = _c.standoffDistance,

            // �Ʒ� �� ��������Ʈ�� Spirit �ʿ��� ���
            despawnOnHit = true,
            onReturned = OnSpiritReturned,
            poolSpawn = _c.poolSpawn // (�ʿ��ϸ� Spirit�� ��ü �罺���� �� ���)
        });

        _actives.Add(sp);
    }

    void OnSpiritReturned(SpiritMinion2D s)
    {
        // ��Ͽ��� ����
        _actives.Remove(s);

        // ���� �ð� ���� ���ϴ� ������ ����
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
