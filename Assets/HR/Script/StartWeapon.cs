using System;
using UnityEngine;

public class StartWeapon : MonoBehaviour
{
    [Header("가판대 무기")]
    public GameObject[] stallWeaponsPrefabs; // 각 가판대 원본 프리팹
    public Transform[] stallTransforms;

    private bool[] playerInRange = new bool[3]; // 플레이어 접근 체크

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
                SetWeaponLayer(obj); // Weapon 레이어 지정
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

        Debug.Log($"선택한 무기: {weaponName}");

        // Player 스크립트 가져오기
        var player = FindObjectOfType<Player>();
        if (player != null && player.weaponHandler != null)
        {
            // WeaponData 불러오기
            WeaponData weaponData = Resources.Load<WeaponData>($"Weapons/{weaponName}");
            if (weaponData != null)
            {
                // Player의 StartingWeapon에 할당
                player.startingWeapon = weaponData;

                // WeaponHandler에 장착
                player.weaponHandler.EquipWeapon(weaponData);
            }
            else
            {
                Debug.LogWarning($"WeaponData not found at Resources/Weapons/{weaponName}");
            }

            // Visual Prefab 불러오기
            GameObject visualPrefab = Resources.Load<GameObject>($"Weapons/{weaponName}");
            if (visualPrefab != null)
            {
                // WeaponHandler에 비주얼 적용
                if (player.weaponHandler.weaponVisualHolder != null)
                {
                    // 기존 비주얼 제거
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

        // 선택 후 가판대 무기 제거
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
