using System;
using UnityEngine;
using System.Linq;

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
        var player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            // Player 자식에서 FirePos, TailPos, WeaponVisualHolder 찾기
            Transform firePos = player.GetComponentsInChildren<Transform>(true)
                                     .FirstOrDefault(t => t.name == "FirePos");
            Transform tailPos = player.GetComponentsInChildren<Transform>(true)
                                     .FirstOrDefault(t => t.name == "TailPos");

            // WeaponHandler 가져오기
            var weaponHandler = player.GetComponentInChildren<WeaponHandler>();
            if (weaponHandler != null)
            {
                // weaponVisualHolder 없으면 생성
/*                if (weaponHandler.weaponVisualHolder == null)
                {
                    GameObject holder = new GameObject("WeaponVisualHolder");
                    holder.transform.SetParent(weaponHandler.transform, false);
                    weaponHandler.weaponVisualHolder = holder.transform;
                }*/

                if (firePos == null)
                {
                    Debug.LogError("FirePos를 찾을 수 없음!");
                    return;
                }

                if (tailPos == null)
                {
                    Debug.LogWarning("TailPos를 찾을 수 없음!");
                }

                // WeaponHandler 초기화
                weaponHandler.Initialize(firePos, weaponHandler.weaponVisualHolder, tailPos);

                // WeaponData 불러오기
                WeaponData weaponData = Resources.Load<WeaponData>($"Weapons/{weaponName}");
                if (weaponData != null)
                {
                    player.startingWeapon = weaponData;
                    weaponHandler.EquipWeapon(weaponData);

                    Debug.Log($"WeaponHandler에 {weaponName} 장착 완료");
                }
                else
                {
                    Debug.LogWarning($"WeaponData not found at Resources/Weapons/{weaponName}");
                }
            }
            else
            {
                Debug.LogError("WeaponHandler를 Player 자식에서 찾을 수 없음!");
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
