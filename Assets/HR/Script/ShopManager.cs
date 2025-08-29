using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Manager;

public class ShopManager : MonoBehaviour
{
    [Header("���� ������ ����Ʈ")]
    public GameObject[] weaponPrefabs;

    [Header("���Ǵ� ��ġ (3��)")]
    public Transform[] stallTransforms; // 3�� ���Ǵ�

    private GameObject[] spawnedWeapons;
    private int[] spawnedWeaponPrefabIndices; // ������ ���� ������ �ε���
    private bool[] playerInRange;
    private float weaponYOffset = 0.5f; // ���� Y�� ������

    //ü�� ����
    public GameObject healthPotionPrefab;

    // ������ ���� ���� �ܺ� ���ٿ�
    public GameObject LastPurchasedWeaponPrefab { get; private set; }
    public int LastPurchasedStallIndex { get; private set; }

    // ���� �̺�Ʈ (�ܺο��� ���� ����)
    public event Action<int, GameObject> OnWeaponPurchased;

    void Start()
    {
        // stallTransforms �ڵ� �Ҵ� (��Ŀ� ����, 2~4��° �ڽĸ�)
        if (stallTransforms == null || stallTransforms.Length == 0)
        {
            stallTransforms = new Transform[3];
            for (int i = 0; i < 3; i++)
            {
                stallTransforms[i] = transform.GetChild(i + 1); // 1,2,3�� �ڽ�
            }
        }

        int stallCount = stallTransforms.Length;
        spawnedWeapons = new GameObject[stallCount];
        spawnedWeaponPrefabIndices = new int[stallCount];
        playerInRange = new bool[stallCount];

        int weaponLayer = LayerMask.NameToLayer("Weapon");
        if (weaponLayer == -1)
        {
            Debug.LogWarning("\"Weapon\" ���̾ �������� �ʽ��ϴ�. �⺻ ���̾�(0)�� ���⸦ �����մϴ�.");
            weaponLayer = 0; // Default layer
        }

        // �ߺ� ������ ���� ����Ʈ
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < weaponPrefabs.Length; i++)
            availableIndices.Add(i);

        int potionStallIndex = UnityEngine.Random.Range(0, stallCount);

for (int i = 0; i < stallCount; i++)
        {
            Vector3 spawnPos = stallTransforms[i].position + new Vector3(0, weaponYOffset, 0);

            if (i == potionStallIndex && healthPotionPrefab != null)
            {
                // ü�� ���� ��ġ
                spawnedWeapons[i] = Instantiate(healthPotionPrefab, spawnPos, Quaternion.identity, stallTransforms[i]);
                spawnedWeaponPrefabIndices[i] = -1; // -1�� ������ �ǹ�
            }
            else
            {
                if (availableIndices.Count == 0)
                {
                    Debug.LogWarning("���Ǵ� ���� ���� �������� ���� �ߺ��� �߻��� �� �ֽ��ϴ�.");
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

        // �׽�Ʈ: MŰ�� HP ����
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (SystemManager.Instance.HpControl != null)
            {
                SystemManager.Instance.HpControl.MinusHp();
                Debug.Log($"HP 1 ����! HP: {SystemManager.Instance.HpControl.CurrentHp}");
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
            Debug.LogWarning("�����Ϸ��� ���Ⱑ �̹� ���ŵǾ��ų� �������� �ʽ��ϴ�.");
            return;
        }

        // �� ���� Ȯ��
        if (GameManager.Manager.PlayerScript.Coins < 25)
        {
            Debug.Log("������ �����մϴ�! 25������ �ʿ��մϴ�.");
            return;
        }

        // ü�� �������� Ȯ��
        if (spawnedWeaponPrefabIndices[index] == -1)
        {
            if (SystemManager.Instance.HpControl != null)
            {
                if (!SystemManager.Instance.HpControl.IsFullHp)
                {
                    SystemManager.Instance.HpControl.PlusHp();
                    Debug.Log($"ü�� ���� ����! HP 1 ���� ���� Hp {SystemManager.Instance.HpControl.CurrentHp}");
                }
                else
                {
                    Debug.Log($"ü���� ���� �� �ֽ��ϴ�! ���� HP {SystemManager.Instance.HpControl.CurrentHp}");
                    return; // ü���� ���� �� ������ ���� �Ұ�
                }
            }
        }
        else
        {
            // ���� ����
            LastPurchasedWeaponPrefab = weaponPrefabs[spawnedWeaponPrefabIndices[index]];
            LastPurchasedStallIndex = index;
            Debug.Log($"���Ǵ� {index + 1} ���� ���� �Ϸ�! ({LastPurchasedWeaponPrefab.name}) ���� ����: {GameManager.Manager.PlayerScript.Coins}");
            OnWeaponPurchased?.Invoke(index, LastPurchasedWeaponPrefab);
        }

        Destroy(spawnedWeapons[index]);

        // ���� ����
        GameManager.Manager.PlayerScript.SpendCoins(25);
    }


    private void SetWeaponSorting(GameObject weaponObj)
    {
        var sr = weaponObj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Weapon"; // ���ϴ� Sorting Layer��
            sr.sortingOrder = 10; // Stall���� ���� ��
        }
        // �ڽ� ������Ʈ���� SpriteRenderer�� �ִٸ� ��� ����
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

    // ��� �ڽı��� ���̾� ����
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
