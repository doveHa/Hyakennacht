using Manager;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerPrefab;
    public WeaponHandler weaponHandler;
    public WeaponData startingWeapon;
    public int Coins { get; private set; }
    public Transform Target { get; private set; }

    void Awake()
    {  
        DontDestroyOnLoad(this);
        Instantiate(playerPrefab, transform).transform.parent = transform;
        Target = transform.GetChild(0).GetChild(1);
        Coins = 0;
    }
    
    void Start()
    {
        weaponHandler.EquipWeapon(startingWeapon);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("무기 사용 시도");
            weaponHandler.UseWeapon();
        }
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