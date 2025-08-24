using Manager;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerPrefab;
    public WeaponHandler WeaponHandler { get; private set; }
    public int Coins { get; private set; }

    void Awake()
    {
        Instantiate(playerPrefab, transform).transform.parent = transform;
        WeaponHandler = GetComponent<WeaponHandler>();
        Coins = 0;
    }
    void Start()
    {
    }

    void Update()
    {
    }

    public void Hit()
    {
        SystemManager.Manager.HpControl.MinusHp();
    }
    public void PlayerGetCoin()
    {
        Coins++;
    }
}