using Character;
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
        var playerSpriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
        bool isLeft = playerSpriteRenderer != null && playerSpriteRenderer.flipX;

        if (weaponHandler != null)
        {
            weaponHandler.UpdateWeaponDirection(isLeft);
            if (weaponHandler is IFlippableWeapon flippable)
            {
                flippable.SetFacingDirection(isLeft);
            }
        }

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

    //HR
    public bool SpendCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            return true;
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.tag.Equals("Projectile"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            Destroy(other.gameObject);
        }
    }
}