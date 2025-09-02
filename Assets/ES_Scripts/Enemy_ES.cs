using UnityEngine;

public class Enemy_ES : MonoBehaviour
{
    EnemyStats _stats;                  // ���� ����/ü��
    Enemy.EnemyController _controller;  

    public int hp => Mathf.RoundToInt(_stats ? _stats.Health : _legacyHp);
    public int maxHp => Mathf.RoundToInt(_stats ? _stats.MaxHealth : _legacyMaxHp);

    public float moveSpeed
    {
        get => _stats ? _stats.Speed : _legacyMoveSpeed;
        set
        {
            if (_stats) _stats.Speed = value;
            _legacyMoveSpeed = value; // ���
        }
    }

    float _legacyHp = 10f, _legacyMaxHp = 10f, _legacyMoveSpeed = 3f;

    void Awake()
    {
        _stats = GetComponent<EnemyStats>();
        _controller = GetComponent<Enemy.EnemyController>();

        if (_stats != null)
        {
            if (_stats.MaxHealth > 0f) { _legacyMaxHp = _stats.MaxHealth; }
            if (_stats.Health > 0f) { _legacyHp = _stats.Health; }
            _legacyMoveSpeed = _stats.Speed;
        }
    }

    public void TakeDamage(int dmg)
    {
        if (_stats != null) _stats.Hurt(dmg);
        else
        {
            _legacyHp = Mathf.Max(0f, _legacyHp - Mathf.Max(0, dmg));
            if (_legacyHp <= 0f) Die();
            else _controller.Animator.SetTrigger("Hit");
        }

        Debug.Log($"{name}��(��) {dmg} �������� �Ծ����ϴ�. ���� ü��: {hp}");
    }

    public void Heal(int amount)
    {
        if (_stats != null) _stats.Heal(amount);
        else
        {
            _legacyHp = Mathf.Min(_legacyMaxHp, _legacyHp + Mathf.Max(0, amount));
        }
        Debug.Log($"{name}��(��) {amount} ȸ��. ���� ü��: {hp}");
    }

    public void Die()
    {
        Debug.Log($"{name}��(��) ����߽��ϴ�.");
        if (_stats != null) _stats.Death();
        else Destroy(gameObject);
    }
}
