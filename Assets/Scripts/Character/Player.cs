using Manager;
using UnityEngine;
using static Constant;

public class Player : MonoBehaviour
{
    public GameObject playerPrefab;
    public WeaponHandler weaponHandler;
    public WeaponData startingWeapon;
    public int Coins { get; private set; }
    public Transform Target { get; private set; }

    private GameObject player;

    void Awake()
    {  
        DontDestroyOnLoad(this);
        //Instantiate(playerPrefab, transform).transform.parent = transform;
        player = Instantiate(playerPrefab, transform);
        Target = transform.GetChild(0).GetChild(1);
        Coins = 0;
    }
    
    void Start()
    {
        weaponHandler = player.GetComponent<WeaponHandler>();
        Transform firePos = player.transform.Find("Body/FirePos");
        Transform tailPos = player.transform.Find("Body/TailPos");

        if (weaponHandler != null && firePos != null && tailPos != null)
        {
            weaponHandler.Initialize(firePos, firePos, tailPos);
            weaponHandler.EquipWeapon(startingWeapon);
        }
        else
        {
            Debug.LogError("무기 초기화 실패: 핸들러 또는 위치 누락");
        }
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