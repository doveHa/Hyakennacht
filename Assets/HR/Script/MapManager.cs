using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq; // for .ToList()??

public class MapManager : MonoBehaviour
{
    [Header("Tilemap References")]
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public TileBase groundTile;
    public TileBase wallTile;

    [Header("Room Settings")]
    public int roomCount = 7;
    public int roomWidth = 13;
    public int roomHeight = 13;

    [Header("Corridor Settings")]
    public int corridorWidth = 3;

    [Header("Stair Tiles")]
    public TileBase stairUpTile;
    public TileBase stairDownTile;

    [Header("Item Spawn")]
    public GameObject[] itemPrefabs;
    public int maxItemCount = 10;

    // 방 바닥 타일 위치 저장용
    private List<Vector3Int> floorTiles = new List<Vector3Int>();

    private Vector2Int[] dirVectors =
    {
        new Vector2Int(0, 1),   // Up
        new Vector2Int(0, -1),  // Down
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(1, 0)    // Right
    };

    private class Room
    {
        public Vector2Int gridPos;
        public List<Vector2Int> connectedRooms = new List<Vector2Int>();
    }

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        // 디버그용: 키를 누르면 맵 재생성
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearItems();
            GenerateMap();
            Debug.Log("Map regenerated");
        }
    }

    void GenerateMap()
    {
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        floorTiles.Clear();

        Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        Vector2Int startPos = Vector2Int.zero;

        Queue<Vector2Int> toExplore = new Queue<Vector2Int>();
        toExplore.Enqueue(startPos);

        rooms[startPos] = new Room { gridPos = startPos };
        occupied.Add(startPos);

        while (rooms.Count < roomCount && toExplore.Count > 0)
        {
            Vector2Int current = toExplore.Dequeue();

            List<Vector2Int> directions = new List<Vector2Int>(dirVectors);
            Shuffle(directions);

            foreach (var dir in directions)
            {
                Vector2Int nextPos = current + dir;

                if (rooms.Count >= roomCount)
                    break;

                if (!occupied.Contains(nextPos))
                {
                    Room newRoom = new Room { gridPos = nextPos };
                    rooms[nextPos] = newRoom;
                    occupied.Add(nextPos);

                    // 서로 연결
                    rooms[current].connectedRooms.Add(nextPos);
                    newRoom.connectedRooms.Add(current);

                    toExplore.Enqueue(nextPos);
                }
                else
                {
                    // 이미 방이 있는데 아직 연결 안 됐으면 연결
                    if (!rooms[current].connectedRooms.Contains(nextPos))
                    {
                        rooms[current].connectedRooms.Add(nextPos);
                        rooms[nextPos].connectedRooms.Add(current);
                    }
                }
            }
        }

        // 방과 복도 바닥 그리기
        foreach (var room in rooms.Values)
        {
            Vector2Int worldPos = room.gridPos * new Vector2Int(roomWidth + corridorWidth, roomHeight + corridorWidth);
            DrawRoomFloor(worldPos, roomWidth, roomHeight);

            foreach (var conn in room.connectedRooms)
            {
                Vector2Int connWorldPos = conn * new Vector2Int(roomWidth + corridorWidth, roomHeight + corridorWidth);
                DrawCorridor(worldPos, connWorldPos);
            }
        }

        // 바닥 타일 모으기 (GenerateWalls 호출 전에)
        CollectFloorTiles();

        // 계단 배치
        PlaceStairs();

        // 아이템 생성
        SpawnItems();

        // 바닥 주변 벽 생성
        GenerateWalls();

    }

    void DrawRoomFloor(Vector2Int worldPos, int width, int height)
    {
        int startX = worldPos.x - width / 2;
        int startY = worldPos.y - height / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePos = new Vector3Int(startX + x, startY + y, 0);
                groundTilemap.SetTile(tilePos, groundTile);

                floorTiles.Add(tilePos);
            }
        }
    }

    void DrawCorridor(Vector2Int from, Vector2Int to)
    {
        Vector2Int dir = new Vector2Int(
            to.x > from.x ? 1 : to.x < from.x ? -1 : 0,
            to.y > from.y ? 1 : to.y < from.y ? -1 : 0
        );

        Vector2Int pos = from;
        while (pos != to)
        {
            for (int w = -corridorWidth / 2; w <= corridorWidth / 2; w++)
            {
                Vector3Int tilePos;
                if (dir.x != 0)
                    tilePos = new Vector3Int(pos.x, pos.y + w, 0);
                else
                    tilePos = new Vector3Int(pos.x + w, pos.y, 0);

                groundTilemap.SetTile(tilePos, groundTile);
                floorTiles.Add(tilePos);
            }
            pos += dir;
        }
    }

    void CollectFloorTiles()
    {
        // floorTiles는 DrawRoomFloor, DrawCorridor에서 계속 채워졌음
        // 혹시 중복 제거 필요하면 아래처럼
        floorTiles = floorTiles.Distinct().ToList();
    }

    void PlaceStairs()
    {
        if (floorTiles.Count < 2) return;

        // 랜덤 2개 뽑기
        var shuffled = floorTiles.OrderBy(x => Random.value).ToList();

        Vector3Int upPos = shuffled[0];
        Vector3Int downPos = shuffled[1];

        groundTilemap.SetTile(upPos, stairUpTile);
        groundTilemap.SetTile(downPos, stairDownTile);
    }

    void SpawnItems()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0) return;

        int spawnCount = Mathf.Min(maxItemCount, floorTiles.Count);
        var shuffled = floorTiles.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3Int tilePos = shuffled[i];
            Vector3 worldPos = groundTilemap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);

            // 아이템 종류 랜덤 선택
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject selectedPrefab = itemPrefabs[randomIndex];

            Instantiate(selectedPrefab, worldPos, Quaternion.identity, this.transform);
        }
    }

    void ClearItems()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Item"))
            {
                Destroy(child.gameObject);
            }
        }
    }


    void GenerateWalls()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        for (int x = bounds.xMin - 1; x <= bounds.xMax + 1; x++)
        {
            for (int y = bounds.yMin - 1; y <= bounds.yMax + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (groundTilemap.GetTile(pos) == null)
                {
                    if (HasGroundNeighbour(pos))
                    {
                        wallTilemap.SetTile(pos, wallTile);
                    }
                }
            }
        }
    }

    bool HasGroundNeighbour(Vector3Int pos)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                if (groundTilemap.GetTile(new Vector3Int(pos.x + dx, pos.y + dy, 0)) != null)
                    return true;
            }
        }
        return false;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }
}
