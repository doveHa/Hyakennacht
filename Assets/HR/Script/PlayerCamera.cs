using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCamera : MonoBehaviour
{
    public Transform player; // �÷��̾��� Transform�� �Ҵ�

    public Vector3 offset = new Vector3(0, 5, -10);

    //public MapManager mapManager; // �ν����Ϳ��� �Ҵ��ϰų� FindObjectOfType ���

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
            transform.LookAt(player);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // ��ȣ�ۿ� Ű
        {
            //TryInteractWithStairs();
            Debug.Log("E key pressed for interaction");
        }
    }

/*    void TryInteractWithStairs()
    {
        // �÷��̾��� ù ��° �ڽ� ������Ʈ ��ġ�� �������� Ÿ�ϸ� ��ǥ ��ȯ
        if (player.childCount == 0)
        {
            Debug.LogWarning("Player�� �ڽ� ������Ʈ�� �����ϴ�.");
            return;
        }

        Transform childTransform = player.GetChild(0); // ù ��° �ڽ�
        Vector3 childWorldPos = childTransform.position;
        Vector3Int tilePos = mapManager.groundTilemap.WorldToCell(childWorldPos);

        // ���� Ÿ���� ��� Ÿ������ Ȯ��
        TileBase currentTile = mapManager.groundTilemap.GetTile(tilePos);
        if (currentTile == mapManager.stairUpTile || currentTile == mapManager.stairDownTile)
        {
            mapManager.NextStage();
            Debug.Log("Interacted with stairs at position: " + tilePos);
        }
    }*/
}