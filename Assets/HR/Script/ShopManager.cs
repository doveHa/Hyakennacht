using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Manager;

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

    //체력 포션
    public GameObject healthPotionPrefab;

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

        // 중복 방지를 위한 리스트
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < weaponPrefabs.Length; i++)
            availableIndices.Add(i);

        int potionStallIndex = UnityEngine.Random.Range(0, stallCount);

for (int i = 0; i < stallCount; i++)
        {
            Vector3 spawnPos = stallTransforms[i].position + new Vector3(0, weaponYOffset, 0);

            if (i == potionStallIndex && healthPotionPrefab != null)
            {
                // 체력 포션 배치
                spawnedWeapons[i] = Instantiate(healthPotionPrefab, spawnPos, Quaternion.identity, stallTransforms[i]);
                spawnedWeaponPrefabIndices[i] = -1; // -1은 포션을 의미
            }
            else
            {
                if (availableIndices.Count == 0)
                {
                    Debug.LogWarning("가판대 수가 무기 종류보다 많아 중복이 발생할 수 있습니다.");
                    break;
                }

                int randListIdx = UnityEngine.Random.Range(0, availableIndices.Count);
                int randIdx = availableIndices[randListIdx];
                availableIndices.RemoveAt(randListIdx);

                spawnedWeapons[i] = Instantiate(weaponPrefabs[randIdx], spawnPos, Quaternion.identity, stallTransforms[i]);
                spawnedWeaponPrefabIndices[i] = randIdx; 
            }

            SetWeaponSorting(spawnedWeapons[i]);
            SetLayerRecursively(spawnedWeapons[i], weaponLayer);
            playerInRange[i] = false;
        }
    }

    void Update()
    {
        for (int i = 0; i < playerInRange.Length; i++)
        {
            if (playerInRange[i] && Input.GetKeyDown(KeyCode.Q))
            {
                BuyWeapon(i);
            }
        }

        // 테스트: M키로 HP 감소
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (SystemManager.Instance.HpControl != null)
            {
                SystemManager.Instance.HpControl.MinusHp();
                Debug.Log($"HP 1 감소! HP: {SystemManager.Instance.HpControl.CurrentHp}");
            }
        }
        
        if(Input.GetKeyDown(KeyCode.N))
        {
            for(int i = 0; i < 20; i++)
                GameManager.Manager.PlayerScript.PlayerGetCoin();
        }
    }

    private void BuyWeapon(int index)
    {
        if (spawnedWeapons[index] == null)
        {
            Debug.LogWarning("구매하려는 무기가 이미 제거되었거나 존재하지 않습니다.");
            return;
        }

        // 맵 코인 확인
        if (GameManager.Manager.PlayerScript.Coins < 25)
        {
            Debug.Log("코인이 부족합니다! 25코인이 필요합니다.");
            return;
        }

        // 체력 포션인지 확인
        if (spawnedWeaponPrefabIndices[index] == -1)
        {
            if (SystemManager.Instance.HpControl != null)
            {
                if (!SystemManager.Instance.HpControl.IsFullHp)
                {
                    SystemManager.Instance.HpControl.PlusHp();
                    Debug.Log($"체력 포션 구매! HP 1 증가 현재 Hp {SystemManager.Instance.HpControl.CurrentHp}");
                }
                else
                {
                    Debug.Log($"체력이 가득 차 있습니다! 현재 HP {SystemManager.Instance.HpControl.CurrentHp}");
                    return; // 체력이 가득 차 있으면 구매 불가
                }
            }
        }
        else
        {
            // 무기 구매
            LastPurchasedWeaponPrefab = weaponPrefabs[spawnedWeaponPrefabIndices[index]];
            LastPurchasedStallIndex = index;
            Debug.Log($"가판대 {index + 1} 무기 구매 완료! ({LastPurchasedWeaponPrefab.name}) 남은 코인: {GameManager.Manager.PlayerScript.Coins}");
            OnWeaponPurchased?.Invoke(index, LastPurchasedWeaponPrefab);
        }

        Destroy(spawnedWeapons[index]);

        // 코인 차감
        GameManager.Manager.PlayerScript.SpendCoins(25);
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
