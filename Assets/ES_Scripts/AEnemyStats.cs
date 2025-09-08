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
        
        AddressableManager.EnemyStat stat = AddressableManager.Manager.GetStatByName(enemyName.ToString());
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
    
}