using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerCamera : MonoBehaviour
{
    public Transform player; // 플레이어 Transform
    public Vector3 offset = new Vector3(0, 5, -10);
    public MapManager mapManager; // Inspector에서 할당

    void Awake()
    {
        // 씬 전환될 때마다 자동 호출되도록 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 메모리 누수 방지
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    void Start()
    {
        FindPlayer();
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
            //TryInteractWithStairs();
            MapUIManager.Instance.OnStageEnd();
        }
    }

    void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player"); // Tag 사용 권장
            if (playerObj != null)
                player = playerObj.transform;
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
        // 플레이어 Collider 하단 기준 위치
        Collider2D col = player.GetComponent<Collider2D>();
        if (col != null)
            return col.bounds.min + Vector3.up * 0.05f; // 약간 위로 offset
        else
            return player.position;
    }

    public void TryInteractWithStairs()
    {

        if (player == null || mapManager == null)
        {
            Debug.LogWarning("Player 또는 MapManager가 할당되지 않았습니다.");
            return;
        }

        Vector3 checkPos = GetPlayerBottomPosition();
        Vector3Int tilePos = mapManager.groundTilemap.WorldToCell(checkPos);

        TileBase currentTile = mapManager.groundTilemap.GetTile(tilePos);

        if (currentTile == mapManager.stairUpTile)
        {
            mapManager.NextStage(true); // 난이도 상승
            Debug.Log("Stairs Up interacted. Moving to next stage.");
        }
        else if (currentTile == mapManager.stairDownTile)
        {
            mapManager.NextStage(false); // 난이도 하락
            Debug.Log("Stairs Down interacted. Moving to previous stage.");
        }
    }
}
