using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("���� ������ ����Ʈ")]
    public GameObject[] weaponPrefabs;

    [Header("���Ǵ� ��ġ (3��)")]
    public Transform[] stallTransforms; // 3�� ���Ǵ�

    private GameObject[] spawnedWeapons;
    private bool[] playerInRange;
    private float weaponYOffset = 0.5f; // ���� Y�� ������

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
                //Debug.Log($"���Ǵ� {i + 1} playerInRange true");
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Debug.Log("QŰ �Է� ������");
                    BuyWeapon(i);
                }
            }
        }
    }

    // 2D ������ Ʈ���� �̺�Ʈ
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player �Ǵ� Player�� �ڽ� ������Ʈ�� �浹�ߴ��� Ȯ��
        Transform root = other.transform.root;
        if (root.CompareTag("Player"))
        {
            for (int i = 0; i < stallTransforms.Length; i++)
            {
                if (other.transform == stallTransforms[i])
                {
                    playerInRange[i] = true;
                    Debug.Log($"�÷��̾ ���Ǵ� {i + 1}�� �����߽��ϴ�. ���⸦ �����Ϸ��� Q Ű�� ��������.");
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
                    Debug.Log($"�÷��̾ ���Ǵ� {i + 1}���� �������ϴ�. ���� ���Ű� �Ұ����մϴ�.");
                }
            }
        }
    }

    private void BuyWeapon(int index)
    {
        Debug.Log($"���Ǵ� {index + 1} ���� ���� �Ϸ�!");
        if (spawnedWeapons[index] != null)
        {
            Destroy(spawnedWeapons[index]);
        }
        else
        {
            Debug.LogWarning("�����Ϸ��� ���Ⱑ �̹� ���ŵǾ��ų� �������� �ʽ��ϴ�.");
        }
    }

    public void SetPlayerInRange(int index, bool value)
    {
        playerInRange[index] = value;
    }
}
