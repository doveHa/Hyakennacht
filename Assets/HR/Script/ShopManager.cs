using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("무기 프리팹 리스트")]
    public GameObject[] weaponPrefabs;

    [Header("가판대 위치 (3개)")]
    public Transform[] stallTransforms; // 3개 가판대

    private GameObject[] spawnedWeapons;
    private bool[] playerInRange;
    private float weaponYOffset = 0.5f; // 무기 Y축 오프셋

    void Start()
    {
        int stallCount = stallTransforms.Length;
        spawnedWeapons = new GameObject[stallCount];
        playerInRange = new bool[stallCount];

        for (int i = 0; i < stallCount; i++)
        {
            int randIdx = Random.Range(0, weaponPrefabs.Length);
            Vector3 spawnPos = stallTransforms[i].position + new Vector3(0, weaponYOffset, 0);
            spawnedWeapons[i] = Instantiate(weaponPrefabs[randIdx], spawnPos, Quaternion.identity, stallTransforms[i]);
            playerInRange[i] = false;
        }
    }

    void Update()
    {
        for (int i = 0; i < playerInRange.Length; i++)
        {
            if (playerInRange[i])
            {
                //Debug.Log($"가판대 {i + 1} playerInRange true");
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Debug.Log("Q키 입력 감지됨");
                    BuyWeapon(i);
                }
            }
        }
    }

    // 2D 물리용 트리거 이벤트
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player 또는 Player의 자식 오브젝트와 충돌했는지 확인
        Transform root = other.transform.root;
        if (root.CompareTag("Player"))
        {
            for (int i = 0; i < stallTransforms.Length; i++)
            {
                if (other.transform == stallTransforms[i])
                {
                    playerInRange[i] = true;
                    Debug.Log($"플레이어가 가판대 {i + 1}에 접근했습니다. 무기를 구매하려면 Q 키를 누르세요.");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Transform root = other.transform.root;
        if (root.CompareTag("Player"))
        {
            for (int i = 0; i < stallTransforms.Length; i++)
            {
                if (other.transform == stallTransforms[i])
                {
                    playerInRange[i] = false;
                    Debug.Log($"플레이어가 가판대 {i + 1}에서 나갔습니다. 무기 구매가 불가능합니다.");
                }
            }
        }
    }

    private void BuyWeapon(int index)
    {
        Debug.Log($"가판대 {index + 1} 무기 구매 완료!");
        if (spawnedWeapons[index] != null)
        {
            Destroy(spawnedWeapons[index]);
        }
        else
        {
            Debug.LogWarning("구매하려는 무기가 이미 제거되었거나 존재하지 않습니다.");
        }
    }

    public void SetPlayerInRange(int index, bool value)
    {
        playerInRange[index] = value;
    }
}
