using System;
using UnityEngine;

public class StartWeapon : MonoBehaviour
{
    [Header("���Ǵ� ����")]
    public GameObject[] stallWeapons = new GameObject[3]; // �� ���Ǵ뿡 ��ġ�� ����
    public Transform[] stallTransforms = new Transform[3]; // 3�� ���Ǵ� ��ġ

    private bool[] playerInRange = new bool[3]; // �÷��̾� ���� üũ

    // ������ ���� ����
    public GameObject SelectedWeapon { get; private set; }
    public int SelectedStallIndex { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (stallWeapons.Length != 3 || stallTransforms.Length != 3)
        {
            Debug.LogError("Stalls or weapons are not properly assigned!");
            return;
        }

        // ���� ����
        for (int i = 0; i < 3; i++)
        {
            if (stallWeapons[i] != null && stallTransforms[i] != null)
            {
                GameObject weaponInstance = Instantiate(
                    stallWeapons[i],
                    stallTransforms[i].position,
                    Quaternion.identity
                );
                // �θ�� ���Ǵ� ���� (�� ������)
                weaponInstance.transform.SetParent(stallTransforms[i]);
                // Weapon ���̾�� Sorting ����
                SetWeaponLayerAndSorting(weaponInstance);
                // ������ ���⸦ �迭�� ���� (���� �� ������)
                stallWeapons[i] = weaponInstance;
            }
        }

    }

    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (playerInRange[i] && Input.GetKeyDown(KeyCode.Q))
            {
                SelectWeapon(i);
            }
        }
    }

    private void SelectWeapon(int index)
    {
        if (stallWeapons[index] == null)
        {
            Debug.LogWarning("�����Ϸ��� ���Ⱑ �������� �ʽ��ϴ�.");
            return;
        }

        SelectedWeapon = stallWeapons[index];
        SelectedStallIndex = index;

        Debug.Log($"���Ǵ� {index + 1}���� {SelectedWeapon.name} ���� �Ϸ�!");

        // ���� �� �ش� ���� ����
        stallWeapons[index] = null;
        Destroy(SelectedWeapon); // �ʿ��ϸ� ������ ����
    }

    // �÷��̾� ���� ���� �ܺ� ����
    public void SetPlayerInRange(int index, bool value)
    {
        if (index < 0 || index >= 3) return;
        playerInRange[index] = value;
    }

    // ���� ���̾�/Sorting ����
    private void SetWeaponLayerAndSorting(GameObject weaponObj)
    {
        if (weaponObj == null) return;

        var sr = weaponObj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Weapon"; // Weapon ���̾� ���� �ʿ�
            sr.sortingOrder = 10;           // ��溸�� ��
        }

        // �ڽ� SpriteRenderer ��� ����
        foreach (var childSr in weaponObj.GetComponentsInChildren<SpriteRenderer>())
        {
            childSr.sortingLayerName = "Weapon";
            childSr.sortingOrder = 10;
        }

        // ���̾ Weapon���� ���� (�浹 ���̾�� �޸� �ʿ� ��)
        SetLayerRecursively(weaponObj, LayerMask.NameToLayer("Weapon"));
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }
}
