using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Manager;
using UnityEngine;
using Enemy;
using UnityEngine.Serialization;

public abstract class AEnemyStats : MonoBehaviour
{
    private static float MAX_HP_COFF = 1;

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
    public float MaxHp;
    private bool _isDead;

    protected virtual void Awake()
    {
        Controller = GetComponent<EnemyController>();

        AddressableManager.EnemyStat stat = AddressableManager.Manager.GetStatByName(enemyName.ToString());
        MaxHp = stat.Health * MAX_HP_COFF;
        CurrentHp = MaxHp;
        Speed = stat.Speed;
    }

    public abstract void TakeDamage(float dmg);

    public void Heal(int amount)
    {
        CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
    }

    public abstract void Die();

    public static void ToHard()
    {
        MAX_HP_COFF += 0.3f;
    }

    public static void ToEasy()
    {
        MAX_HP_COFF = Mathf.Max(0, MAX_HP_COFF - 0.3f);
    }

    public static void LevelInitialize()
    {
        MAX_HP_COFF = 1;
    }
}