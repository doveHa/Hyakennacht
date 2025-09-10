using UnityEngine;

public class BossPrefab : MonoBehaviour
{
    public static BossPrefab Instance;
    public GameObject[] BossPrefabs;
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
