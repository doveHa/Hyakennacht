using System;
using UnityEngine;

public class StartWeapon : MonoBehaviour
{
    [Header("가판대 무기")]
    public GameObject[] stallWeapons = new GameObject[3]; // 각 가판대에 배치된 무기
    public Transform[] stallTransforms = new Transform[3]; // 3개 가판대 위치

    private bool[] playerInRange = new bool[3]; // 플레이어 접근 체크

    // 선택한 무기 저장
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

        // 무기 생성
        for (int i = 0; i < 3; i++)
        {
            if (stallWeapons[i] != null && stallTransforms[i] != null)
            {
                GameObject weaponInstance = Instantiate(
                    stallWeapons[i],
                    stallTransforms[i].position,
                    Quaternion.identity
                );
                // 부모로 가판대 지정 (씬 정리용)
                weaponInstance.transform.SetParent(stallTransforms[i]);
                // Weapon 레이어와 Sorting 적용
                SetWeaponLayerAndSorting(weaponInstance);
                // 생성된 무기를 배열에 저장 (선택 시 참조용)
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
            Debug.LogWarning("선택하려는 무기가 존재하지 않습니다.");
            return;
        }

        SelectedWeapon = stallWeapons[index];
        SelectedStallIndex = index;

        Debug.Log($"가판대 {index + 1}에서 {SelectedWeapon.name} 선택 완료!");

        // 선택 후 해당 무기 제거
        stallWeapons[index] = null;
        Destroy(SelectedWeapon); // 필요하면 씬에서 제거
    }

    // 플레이어 접근 범위 외부 설정
    public void SetPlayerInRange(int index, bool value)
    {
        if (index < 0 || index >= 3) return;
        playerInRange[index] = value;
    }

    // 무기 레이어/Sorting 적용
    private void SetWeaponLayerAndSorting(GameObject weaponObj)
    {
        if (weaponObj == null) return;

        var sr = weaponObj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Weapon"; // Weapon 레이어 생성 필요
            sr.sortingOrder = 10;           // 배경보다 위
        }

        // 자식 SpriteRenderer 재귀 적용
        foreach (var childSr in weaponObj.GetComponentsInChildren<SpriteRenderer>())
        {
            childSr.sortingLayerName = "Weapon";
            childSr.sortingOrder = 10;
        }

        // 레이어도 Weapon으로 변경 (충돌 레이어와 달리 필요 시)
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
