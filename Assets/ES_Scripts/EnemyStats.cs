using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Manager;
using UnityEngine;
using Enemy;

public class EnemyStats : MonoBehaviour
{
    private enum EnemyName
    {
        Ghost,
        Golem,
        Kappa,
        Slime,
        Straw,
        Wisp,
        Boss1
    }

    [SerializeField] private EnemyName name;

    public float Speed { get; set; }

    private EnemyController _controller;
    private float _currentHp, _maxHp;

    private bool _isDead;

    void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }

    void Update()
    {
    }

    public async Task SetStat()
    {
        TextAsset textAsset = await AddressableManager.Manager.LoadAsset<TextAsset>("Assets/TextAsset/EnemyStats.json");
        OnJsonLoaded(textAsset);
    }

    private void OnJsonLoaded(TextAsset textAsset)
    {
        string json = textAsset.text;

        List<EnemyStat> set = JsonSerializer.Deserialize<List<EnemyStat>>(json);
        EnemyStat stat = set.Find(e => e.Name == name.ToString());
        _maxHp = stat.Health;
        _currentHp = _maxHp;
        Speed = stat.Speed;
    }

    public void TakeDamage(int dmg)
    {
        _controller.ChangeState(new HitState(_controller,_controller.CurrentState));
        _currentHp -= dmg;
        Debug.Log(_currentHp);
        if (_currentHp <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        _currentHp = Mathf.Min(_maxHp, _currentHp + amount);
    }

    public void Die()
    {
        _controller.Animator.SetTrigger("Death");
        GetComponentInParent<EnemySpawner>().KillCount++;
        Destroy(GetComponentInChildren<PlayerRecognize>());
        Destroy(GetComponent<EnemyController>());
        foreach (Collider2D cd in GetComponentsInChildren<Collider2D>())
        {
            Destroy(cd);
        }
        
        Destroy(GetComponent<EnemyStats>());
    }

    private class EnemyStat
    {
        public string Name { get; set; }
        public float Speed { get; set; }
        public float Health { get; set; }
    }
}