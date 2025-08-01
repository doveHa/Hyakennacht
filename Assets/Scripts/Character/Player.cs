using Manager;
using UnityEngine;

public class Player : MonoBehaviour
{
    public WeaponHandler WeaponHandler { get; private set; }
    public int Coins { get; private set; }

    void Awake()
    {
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
        CanvasManager.Manager.hpControl.MinusHp();
    }
    public void PlayerGetCoin()
    {
        Coins++;
    }
}