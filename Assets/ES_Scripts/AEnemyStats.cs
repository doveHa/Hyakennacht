using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Manager;
using UnityEngine;
using Enemy;
using UnityEngine.Serialization;

public abstract class AEnemyStats : MonoBehaviour
{
    private enum EnemyName
    {
        Ghost,
        Golem,
        Kappa,
        Slime,
        Straw,
        Wisp,
        Boss1,
        Boss2,
        MiddleBoss,
        WitchBoss
    }

    [SerializeField] private EnemyName enemyName;

    public float Speed { get; set; }

    protected EnemyController Controller;
    protected float CurrentHp;
    protected float MaxHp;
    private bool _isDead;

    protected virtual void Awake()
    {
        Controller = GetComponent<EnemyController>();
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
        EnemyStat stat = set.Find(e => e.Name == enemyName.ToString());
        MaxHp = stat.Health;
        CurrentHp = MaxHp;
        Speed = stat.Speed;
    }

    public abstract void TakeDamage(float dmg);

    public void Heal(int amount)
    {
        CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
    }

    public abstract void Die();

    private class EnemyStat
    {
        public string Name { get; set; }
        public float Speed { get; set; }
        public float Health { get; set; }
    }
}