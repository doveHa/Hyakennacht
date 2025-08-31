using System;
using UnityEngine;
using System.Linq;

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
        var player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            // Player �ڽĿ��� FirePos, TailPos, WeaponVisualHolder ã��
            Transform firePos = player.GetComponentsInChildren<Transform>(true)
                                     .FirstOrDefault(t => t.name == "FirePos");
            Transform tailPos = player.GetComponentsInChildren<Transform>(true)
                                     .FirstOrDefault(t => t.name == "TailPos");

            // WeaponHandler ��������
            var weaponHandler = player.GetComponentInChildren<WeaponHandler>();
            if (weaponHandler != null)
            {
                // weaponVisualHolder ������ ����
/*                if (weaponHandler.weaponVisualHolder == null)
                {
                    GameObject holder = new GameObject("WeaponVisualHolder");
                    holder.transform.SetParent(weaponHandler.transform, false);
                    weaponHandler.weaponVisualHolder = holder.transform;
                }*/

                if (firePos == null)
                {
                    Debug.LogError("FirePos�� ã�� �� ����!");
                    return;
                }

                if (tailPos == null)
                {
                    Debug.LogWarning("TailPos�� ã�� �� ����!");
                }

                // WeaponHandler �ʱ�ȭ
                weaponHandler.Initialize(firePos, weaponHandler.weaponVisualHolder, tailPos);

                // WeaponData �ҷ�����
                WeaponData weaponData = Resources.Load<WeaponData>($"Weapons/{weaponName}");
                if (weaponData != null)
                {
                    player.startingWeapon = weaponData;
                    weaponHandler.EquipWeapon(weaponData);

                    Debug.Log($"WeaponHandler�� {weaponName} ���� �Ϸ�");
                }
                else
                {
                    Debug.LogWarning($"WeaponData not found at Resources/Weapons/{weaponName}");
                }
            }
            else
            {
                Debug.LogError("WeaponHandler�� Player �ڽĿ��� ã�� �� ����!");
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
