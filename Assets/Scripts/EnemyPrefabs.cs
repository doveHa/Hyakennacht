using UnityEngine;

public class EnemyPrefabs : MonoBehaviour
{
    public static EnemyPrefabs Instance;
    public GameObject[] EnemyPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
