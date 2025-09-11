using System;
using UnityEngine;
using System.Linq;

public class StartWeapon : MonoBehaviour, IInteractable
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
/*        for (int i = 0; i < 3; i++)
        {
            if (playerInRange[i] && Input.GetKeyDown(KeyCode.Q))
            {
                SelectWeapon(i);
            }
        }*/  //Player에서 관리
    }

    // IInteractable 인터페이스 구현
    public void Interact(int index)
    {
        SelectWeapon(index);
    }

    /*    private void SelectWeapon(int index)
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
        }*/

    private void SelectWeapon(int index)
    {
        if (spawnedWeapons[index] == null) return;

        var player = FindAnyObjectByType<Player>();
        if (player == null) return;

        var weaponHandler = player.weaponHandler;
        if (weaponHandler == null) return;

        // 플레이어가 이미 무기를 들고 있는지 확인합니다.
        if (weaponHandler.currentVisual != null)
        {
            WeaponData droppedWeaponData = weaponHandler.GetCurrentWeaponData();
            if (droppedWeaponData != null)
            {
                // 1. 기존 무기를 드랍할 위치를 플레이어 위치로 설정합니다.
                Vector3 dropPosition = player.transform.position;

                // 2. WeaponPickup 스크립트와 콜라이더가 포함된 "드랍용 무기 프리팹"을 생성합니다.
                //    이 프리팹은 게임에 맞게 미리 준비되어 있어야 합니다.
                //    예시: droppedWeaponData.visualPrefab
                GameObject droppedWeaponObj = Instantiate(droppedWeaponData.visualPrefab, dropPosition, Quaternion.identity);

                // 3. 생성된 오브젝트에 WeaponPickup 스크립트를 추가하고 WeaponData를 설정합니다.
                //    (만약 프리팹에 스크립트가 이미 있다면 이 부분은 필요 없을 수 있습니다.)
                var pickupScript = droppedWeaponObj.GetComponent<WeaponPickup>();
                if (pickupScript == null)
                {
                    pickupScript = droppedWeaponObj.AddComponent<WeaponPickup>();
                }
                pickupScript.SetWeaponData(droppedWeaponData);
            }
        }

        // 4. 새로운 무기로 교체합니다.
        string weaponName = spawnedWeapons[index].name.Replace("(Clone)", "").Trim();
        WeaponData newData = Resources.Load<WeaponData>($"Weapons/{weaponName}");
        if (newData != null)
        {
            weaponHandler.EquipWeapon(newData);
            player.startingWeapon = newData;
            Debug.Log($"WeaponHandler에 {weaponName} 장착 완료");
        }
        else
        {
            Debug.LogWarning($"WeaponData not found at Resources/Weapons/{weaponName}");
        }

        // 5. 가판대의 무기를 제거합니다.
        Destroy(spawnedWeapons[index]);
        spawnedWeapons[index] = null;
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
