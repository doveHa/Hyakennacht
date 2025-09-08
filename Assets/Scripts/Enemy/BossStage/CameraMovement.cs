using Manager;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    public Tilemap map;           
    private Transform player;       
    private Vector3 minBounds;    
    private Vector3 maxBounds;     

    private float halfHeight;
    private float halfWidth;
    private Camera cam;

    void Start()
    {
        player = GameManager.Manager.Player.transform;
        cam = GetComponent<Camera>();

        Bounds mapBounds = map.localBounds;
        minBounds = mapBounds.min;
        maxBounds = mapBounds.max;

        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 플레이어 위치 따라가기
        Vector3 targetPos = player.position;

        // 카메라의 이동 범위를 제한
        float clampedX = Mathf.Clamp(targetPos.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float clampedY = Mathf.Clamp(targetPos.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}