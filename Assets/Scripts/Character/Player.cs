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
            Debug.LogError("���� �ʱ�ȭ ����: �ڵ鷯 �Ǵ� ��ġ ����");
        }
        weaponHandler.EquipWeapon(startingWeapon);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("���� ��� �õ�");
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