using System;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("무기 프리팹 리스트")]
    public GameObject[] weaponPrefabs;

    [Header("가판대 위치 (3개)")]
    public Transform[] stallTransforms; // 3개 가판대

    private GameObject[] spawnedWeapons;
    private int[] spawnedWeaponPrefabIndices; // 생성된 무기 프리팹 인덱스
    private bool[] playerInRange;
    private float weaponYOffset = 0.5f; // 무기 Y축 오프셋

    // 구매한 무기 정보 외부 접근용
    public GameObject LastPurchasedWeaponPrefab { get; private set; }
    public int LastPurchasedStallIndex { get; private set; }

    // 구매 이벤트 (외부에서 구독 가능)
    public event Action<int, GameObject> OnWeaponPurchased;

    void Start()
    {
        // stallTransforms 자동 할당 (장식용 제외, 2~4번째 자식만)
        if (stallTransforms == null || stallTransforms.Length == 0)
        {
            stallTransforms = new Transform[3];
            for (int i = 0; i < 3; i++)
            {
                stallTransforms[i] = transform.GetChild(i + 1); // 1,2,3번 자식
            }
        }

        int stallCount = stallTransforms.Length;
        spawnedWeapons = new GameObject[stallCount];
        spawnedWeaponPrefabIndices = new int[stallCount];
        playerInRange = new bool[stallCount];

        int weaponLayer = LayerMask.NameToLayer("Weapon");
        if (weaponLayer == -1)
        {
            Debug.LogWarning("\"Weapon\" 레이어가 존재하지 않습니다. 기본 레이어(0)로 무기를 생성합니다.");
            weaponLayer = 0; // Default layer
        }

        for (int i = 0; i < stallCount; i++)
        {
            int randIdx = UnityEngine.Random.Range(0, weaponPrefabs.Length);
            Vector3 spawnPos = stallTransforms[i].position + new Vector3(0, weaponYOffset, 0);
            spawnedWeapons[i] = Instantiate(weaponPrefabs[randIdx], spawnPos, Quaternion.identity, stallTransforms[i]);
            SetWeaponSorting(spawnedWeapons[i]);
            spawnedWeaponPrefabIndices[i] = randIdx; // 생성된 프리팹 인덱스 저장
            SetLayerRecursively(spawnedWeapons[i], weaponLayer);
            playerInRange[i] = false;
        }
    }

    void Update()
    {
        for (int i = 0; i < playerInRange.Length; i++)
        {
            if (playerInRange[i])
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    BuyWeapon(i);
                }
            }
        }
    }

    private void BuyWeapon(int index)
    {
        if (spawnedWeapons[index] != null)
        {
            // 구매한 무기 정보 저장 (정확한 프리팹 참조)
            LastPurchasedWeaponPrefab = weaponPrefabs[spawnedWeaponPrefabIndices[index]];
            LastPurchasedStallIndex = index;

            Debug.Log($"가판대 {index + 1} 무기 구매 완료! ({LastPurchasedWeaponPrefab.name})");

            // 구매 이벤트 호출
            OnWeaponPurchased?.Invoke(index, LastPurchasedWeaponPrefab);

            Destroy(spawnedWeapons[index]);
        }
        else
        {
            Debug.LogWarning("구매하려는 무기가 이미 제거되었거나 존재하지 않습니다.");
        }
    }

    private void SetWeaponSorting(GameObject weaponObj)
    {
        var sr = weaponObj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Weapon"; // 원하는 Sorting Layer명
            sr.sortingOrder = 10; // Stall보다 높은 값
        }
        // 자식 오브젝트에도 SpriteRenderer가 있다면 모두 변경
        foreach (var childSr in weaponObj.GetComponentsInChildren<SpriteRenderer>())
        {
            childSr.sortingLayerName = "Weapon";
            childSr.sortingOrder = 10;
        }
    }

    public void SetPlayerInRange(int index, bool value)
    {
        playerInRange[index] = value;
    }

    // 모든 자식까지 레이어 변경
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
