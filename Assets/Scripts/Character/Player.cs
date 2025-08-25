using Manager;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public WeaponHandler WeaponHandler { get; private set; }
    public int Coins { get; private set; }

    public WeaponHandler weaponHandler;
    //public WeaponData startingWeapon;

    void Awake()
    {
        weaponHandler = GetComponent<WeaponHandler>();
        Coins = 0;
    }
    void Start()
    {
        //weaponHandler.EquipWeapon(startingWeapon);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("무기 사용 시도");
            weaponHandler.UseWeapon();
        }
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