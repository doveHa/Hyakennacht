using Character;
using Manager;
using UnityEngine;
using static Constant;
using UnityEngine.Tilemaps;

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

    // HR: 계단 상호작용을 시도하는 함수
    public void TryInteractWithStairs()
    {
        if (mapManager == null)
        {
            Debug.LogWarning("MapManager가 할당되지 않았습니다.");
            return;
        }

        Vector3Int playerCellPosition = mapManager.groundTilemap.WorldToCell(transform.position);
        TileBase currentTile = mapManager.groundTilemap.GetTile(playerCellPosition);

        if (currentTile == mapManager.stairUpTile)
        {
            MapManager.NextStage(true);
            Debug.Log("계단(위)과 상호작용: 다음 스테이지로 이동");
        }
        else if (currentTile == mapManager.stairDownTile)
        {
            MapManager.NextStage(false);
            Debug.Log("계단(아래)과 상호작용: 이전 스테이지로 이동");
        }
        else
        {
            Debug.Log("계단 위에 있지 않습니다. 상호작용 불가");
        }
    }
}