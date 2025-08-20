using Manager;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerPrefab;
    public WeaponHandler WeaponHandler { get; private set; }
    public int Coins { get; private set; }

    void Awake()
    {
        WeaponHandler = GetComponent<WeaponHandler>();
        Coins = 0;
    }
    void Start()
    {
        Instantiate(playerPrefab, transform).transform.parent = transform;
    }

    void Update()
    {
    }

    public void Hit()
    {
        SystemManager.Manager.hpControl.MinusHp();
    }
    public void PlayerGetCoin()
    {
        Coins++;
    }
}