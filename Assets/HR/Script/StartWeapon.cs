using System;
using UnityEngine;

public class StartWeapon : MonoBehaviour
{
    [Header("���Ǵ� ����")]
    public GameObject[] stallWeaponsPrefabs; // �� ���Ǵ� ���� ������
    public Transform[] stallTransforms;

    private bool[] playerInRange = new bool[3]; // �÷��̾� ���� üũ

    private GameObject[] spawnedWeapons;

    public GameObject SelectedWeapon { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (stallWeaponsPrefabs.Length != 3 || stallTransforms.Length != 3)
        {
            Debug.LogError("Stalls or weapons are not properly assigned!");
            return;
        }

        spawnedWeapons = new GameObject[3];

        for (int i = 0; i < 3; i++)
        {
            if (stallWeaponsPrefabs[i] != null && stallTransforms[i] != null)
            {
                GameObject obj = Instantiate(stallWeaponsPrefabs[i], stallTransforms[i].position, Quaternion.identity);
                SetWeaponLayer(obj); // Weapon ���̾� ����
                spawnedWeapons[i] = obj;
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
        if (spawnedWeapons[index] == null) return;

        SelectedWeapon = spawnedWeapons[index];
        string weaponName = SelectedWeapon.name.Replace("(Clone)", "").Trim();

        Debug.Log($"������ ����: {weaponName}");

        // Player ��ũ��Ʈ ��������
        var player = FindObjectOfType<Player>();
        if (player != null && player.weaponHandler != null)
        {
            // WeaponData �ҷ�����
            WeaponData weaponData = Resources.Load<WeaponData>($"Weapons/{weaponName}");
            if (weaponData != null)
            {
                // Player�� StartingWeapon�� �Ҵ�
                player.startingWeapon = weaponData;

                // WeaponHandler�� ����
                player.weaponHandler.EquipWeapon(weaponData);
            }
            else
            {
                Debug.LogWarning($"WeaponData not found at Resources/Weapons/{weaponName}");
            }

            // Visual Prefab �ҷ�����
            GameObject visualPrefab = Resources.Load<GameObject>($"Weapons/{weaponName}");
            if (visualPrefab != null)
            {
                // WeaponHandler�� ���־� ����
                if (player.weaponHandler.weaponVisualHolder != null)
                {
                    // ���� ���־� ����
                    foreach (Transform child in player.weaponHandler.weaponVisualHolder)
                        Destroy(child.gameObject);

                    GameObject visual = Instantiate(visualPrefab, player.weaponHandler.weaponVisualHolder);
                    visual.transform.localPosition = Vector3.zero;
                    visual.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                Debug.LogWarning($"Visual prefab not found at Resources/Weapons/{weaponName}");
            }
        }

        // ���� �� ���Ǵ� ���� ����
        spawnedWeapons[index] = null;
        Destroy(SelectedWeapon);
    }


    private void SetWeaponLayer(GameObject obj)
    {
        int layer = LayerMask.NameToLayer("Weapon");
        if (layer == -1) layer = 0;

        obj.layer = layer;
        foreach (Transform t in obj.GetComponentsInChildren<Transform>())
            t.gameObject.layer = layer;
    }

    public void SetPlayerInRange(int index, bool value)
    {
        if (index < 0 || index >= 3) return;
        playerInRange[index] = value;
    }
}
