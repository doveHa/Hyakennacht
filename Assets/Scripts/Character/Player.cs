using Character;
using Manager;
using UnityEngine;
using static Constant;
using UnityEngine.Tilemaps;

//HR: 플레이어와 상호작용 가능한 스크립트의 인터페이스를 정의
public interface IInteractable
{
    void Interact(int index);
}

public class Player : MonoBehaviour
{
    public GameObject playerPrefab;
    public WeaponHandler weaponHandler;
    public WeaponData startingWeapon;
    public int Coins { get; private set; }
    public Transform Target { get; private set; }

    private GameObject player;

    //HR
    public MapManager mapManager;
    private IInteractable currentInteractable;
    // 현재 플레이어가 서 있는 가판대의 인덱스를 저장할 변수를 추가
    private int currentStallIndex = -1;

    void Awake()
    {
        DontDestroyOnLoad(this);
        //Instantiate(playerPrefab, transform).transform.parent = transform;
        player = Instantiate(playerPrefab, transform);
        player.name = "Character";
        Target = transform.Find("Character/Shadow");
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

        // HR: MapManager 인스턴스를 찾아 할당
        if (mapManager == null)
        {
            mapManager = FindFirstObjectByType<MapManager>();
        }
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
        /*
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    Debug.Log("���� ��� �õ�");
                    weaponHandler.UseWeapon();
                }
         */

        // HR: Q 키 입력은 여기서만 처리
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact(currentStallIndex);
            }
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

    // 이 메소드는 다른 스크립트의 OnTriggerEnter2D에서 호출
    public void SetCurrentInteractable(IInteractable interactable, int index)
    {
        currentInteractable = interactable;
        currentStallIndex = index;
    }

    public void ClearCurrentInteractable()
    {
        currentInteractable = null;
        currentStallIndex = -1;
    }

    // 이 메소드는 인덱스가 필요 없는 상호작용 (예: WeaponPickup)을 위해 추가된 오버로드입니다.
    // 이전에 복사된 중복 코드를 삭제하세요.
    public void SetCurrentInteractable(IInteractable interactable)
    {
        SetCurrentInteractable(interactable, -1);
    }
}