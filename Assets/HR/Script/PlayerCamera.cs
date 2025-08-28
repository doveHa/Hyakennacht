using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCamera : MonoBehaviour
{
    public Transform player; // �÷��̾� Transform
    public Vector3 offset = new Vector3(0, 5, -10);
    public MapManager mapManager; // Inspector���� �Ҵ�

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteractWithStairs();
        }
    }

    private bool IsOnStairs()
    {
        if (player == null || mapManager == null) return false;

        Vector3 checkPos = GetPlayerBottomPosition();
        Vector3Int tilePos = mapManager.groundTilemap.WorldToCell(checkPos);

        TileBase currentTile = mapManager.groundTilemap.GetTile(tilePos);
        return (currentTile == mapManager.stairUpTile || currentTile == mapManager.stairDownTile);
    }

    private Vector3 GetPlayerBottomPosition()
    {
        // �÷��̾� Collider �ϴ� ���� ��ġ
        Collider2D col = player.GetComponent<Collider2D>();
        if (col != null)
            return col.bounds.min + Vector3.up * 0.05f; // �ణ ���� offset
        else
            return player.position;
    }

    public void TryInteractWithStairs()
    {
        if (player == null || mapManager == null)
        {
            Debug.LogWarning("Player �Ǵ� MapManager�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        Vector3 checkPos = GetPlayerBottomPosition();
        Vector3Int tilePos = mapManager.groundTilemap.WorldToCell(checkPos);

        TileBase currentTile = mapManager.groundTilemap.GetTile(tilePos);

        if (currentTile == mapManager.stairUpTile)
        {
            mapManager.NextStage(true); // ���̵� ���
            Debug.Log("Stairs Up interacted. Moving to next stage.");
        }
        else if (currentTile == mapManager.stairDownTile)
        {
            mapManager.NextStage(false); // ���̵� �϶�
            Debug.Log("Stairs Down interacted. Moving to previous stage.");
        }
    }
}
