using UnityEngine;

public class DoomDebuff : MonoBehaviour
{
    float _endTime;
    bool _armed;
    string _bubbleFxKey;
    System.Action<string, Vector2> _fx;

    public void Arm(float seconds, string bubbleFxKey, System.Action<string, Vector2> fx)
    {
        _endTime = Time.time + Mathf.Max(0.05f, seconds);
        _bubbleFxKey = bubbleFxKey;
        _fx = fx;
        _armed = true;

        if (!string.IsNullOrEmpty(_bubbleFxKey))
            _fx?.Invoke(_bubbleFxKey, (Vector2)transform.position);
    }

    void Update()
    {
        if (!_armed) return;
        if (Time.time >= _endTime)
        {
            _armed = false;
            var enemy = GetComponent<AEnemyStats>();
            if (enemy != null) enemy.Die();     // ���
            // ������� 1ȸ�� �� �ڽ� �����ص� OK
            Destroy(this);
        }
    }
}

public class PoisonApplePayload : MonoBehaviour, IThrownPayload
{
    [Header("Config")]
    public float doomSeconds = 3f;       // N�� �� ���
    public string bubbleFxKey;           // �� ��ǰ FX

    public void OnImpact(Transform owner, Collider2D hit, Vector2 point)
    {
        var enemy = hit.GetComponentInParent<AEnemyStats>();
        if (!enemy) return;

        var doom = enemy.GetComponent<DoomDebuff>();
        if (!doom) doom = enemy.gameObject.AddComponent<DoomDebuff>();

        System.Action<string, Vector2> fx = null;
        if (owner)
        {
            var caster = owner.GetComponent<SkillCaster>();
            if (caster && caster.fx != null)
                fx = caster.fx.PlayAt;
        }

        doom.Arm(doomSeconds, bubbleFxKey, fx);
    }
}

