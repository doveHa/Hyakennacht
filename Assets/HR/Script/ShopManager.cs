using System;
using UnityEngine;

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

        for (int i = 0; i < stallCount; i++)
        {
            int randIdx = UnityEngine.Random.Range(0, weaponPrefabs.Length);
            Vector3 spawnPos = stallTransforms[i].position + new Vector3(0, weaponYOffset, 0);
            spawnedWeapons[i] = Instantiate(weaponPrefabs[randIdx], spawnPos, Quaternion.identity, stallTransforms[i]);
            SetWeaponSorting(spawnedWeapons[i]);
            spawnedWeaponPrefabIndices[i] = randIdx; // ������ ������ �ε��� ����
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
            // ������ ���� ���� ���� (��Ȯ�� ������ ����)
            LastPurchasedWeaponPrefab = weaponPrefabs[spawnedWeaponPrefabIndices[index]];
            LastPurchasedStallIndex = index;

            Debug.Log($"���Ǵ� {index + 1} ���� ���� �Ϸ�! ({LastPurchasedWeaponPrefab.name})");

            // ���� �̺�Ʈ ȣ��
            OnWeaponPurchased?.Invoke(index, LastPurchasedWeaponPrefab);

            Destroy(spawnedWeapons[index]);
        }
        else
        {
            Debug.LogWarning("�����Ϸ��� ���Ⱑ �̹� ���ŵǾ��ų� �������� �ʽ��ϴ�.");
        }
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
