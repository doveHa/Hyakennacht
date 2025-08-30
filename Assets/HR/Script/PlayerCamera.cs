using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCamera : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform을 할당

    public Vector3 offset = new Vector3(0, 5, -10);

    public MapManager mapManager; // 인스펙터에서 할당하거나 FindObjectOfType 사용

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
        if (Input.GetKeyDown(KeyCode.E)) // 상호작용 키
        {
            TryInteractWithStairs();
            Debug.Log("E key pressed for interaction");
        }
    }

    void TryInteractWithStairs()
    {
        // 플레이어의 첫 번째 자식 오브젝트 위치를 기준으로 타일맵 좌표 변환
        if (player.childCount == 0)
        {
            Debug.LogWarning("Player의 자식 오브젝트가 없습니다.");
            return;
        }

        Transform childTransform = player.GetChild(0); // 첫 번째 자식
        Vector3 childWorldPos = childTransform.position;
        Vector3Int tilePos = mapManager.groundTilemap.WorldToCell(childWorldPos);

        // 현재 타일이 계단 타일인지 확인
        TileBase currentTile = mapManager.groundTilemap.GetTile(tilePos);
        // 계단 종류에 따라 MapManager.NextStage() 호출
        if (currentTile == mapManager.stairUpTile)
        {
            mapManager.NextStage(true); // true는 난이도 상승 (stairUp)
            Debug.Log("Stairs Up interacted. Moving to next stage.");
        }
        else if (currentTile == mapManager.stairDownTile)
        {
            mapManager.NextStage(false); // false는 난이도 하락 (stairDown)
            Debug.Log("Stairs Down interacted. Moving to previous stage.");
        }
    }
}