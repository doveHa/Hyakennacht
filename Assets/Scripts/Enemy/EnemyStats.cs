using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text.Json;

public class EnemyStats : MonoBehaviour
{
    public string name;
    private double _health;
    private double _speed;

    void Awake()
    {
        StreamReader reader =
            new StreamReader(new FileStream("E:\\Project\\Unity\\Hyakennacht\\Assets\\Resources\\Json\\EnemyStats.json",
                FileMode.Open));
        List<EnemyStat> set = JsonSerializer.Deserialize<List<EnemyStat>>(reader.ReadToEnd());
        Debug.Log(set.Count);
    }

    void Start()
    {
    }

    void Update()
    {
    }

    void Hurt(double damage)
    {
        if ((_health - damage) <= 0)
        {
            Death();
        }
        else
        {
            _health -= damage;
        }
    }

    private void Death()
    {
    }

    private class EnemyStat
    {
        public string name;
        public double speed;
        public double Health;
    }
}