using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [Header("Tilemap References")]
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    //public TileBase groundTile;
    // 기존 groundTile 대신 배열로 선언
    public TileBase[] groundTilesByStage;
    public TileBase wallTile;

    [Header("Stage Settings")]
    public int currentStage = 0; // 현재 스테이지

    [Header("Room Settings")]
    public int roomCount = 7;
    public int roomWidth = 13;
    public int roomHeight = 13;

    [Header("Spacing Settings")]
    public int roomSpacing = 25;

    [Header("Corridor Settings")]
    public int corridorWidth = 3;

    [Header("Stair Tiles")]
    public TileBase stairUpTile;
    public TileBase stairDownTile;

    [Header("Item Spawn")]
    public GameObject[] itemPrefabs;
    public int maxItemCount = 10;

    // 모든 바닥 타일
    private List<Vector3Int> groundTiles = new List<Vector3Int>();

    const int maxStage = 15; // 전체 스테이지 수
    const int specialStageCount = 2; // 특수 스테이지 수


    // 방 데이터 구조
    [System.Serializable]
    public class Room
    {
        public int id;
        public Vector2Int gridPos; // 맵 배치 좌표
        public List<Vector3Int> tiles = new List<Vector3Int>(); // 방 내부 타일
        public List<Vector2Int> connectedRooms = new List<Vector2Int>();
    }

    private List<Room> rooms = new List<Room>();

    private Vector2Int[] dirVectors =
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0)
    };

    void Start()
    {
        // 현재 무슨 스테이지인지 StageManager에서 가져옴
        currentStage = StageManager.CurrentStage;
        GenerateMap();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearItems();
            //GenerateMap();
            NextStage();
            Debug.Log("Map regenerated");
        }
    }

    void GenerateMap()
    {
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        groundTiles.Clear();
        rooms.Clear();

        Dictionary<Vector2Int, Room> roomDict = new Dictionary<Vector2Int, Room>();
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        Vector2Int startPos = Vector2Int.zero;
        Queue<Vector2Int> toExplore = new Queue<Vector2Int>();
        toExplore.Enqueue(startPos);

        Room startRoom = new Room { id = 0, gridPos = startPos };
        roomDict[startPos] = startRoom;
        rooms.Add(startRoom);
        occupied.Add(startPos);

        int roomIdCounter = 1;

        while (rooms.Count < roomCount && toExplore.Count > 0)
        {
            Vector2Int current = toExplore.Dequeue();
            List<Vector2Int> directions = new List<Vector2Int>(dirVectors);
            Shuffle(directions);

            foreach (var dir in directions)
            {
                Vector2Int nextPos = current + dir;
                if (rooms.Count >= roomCount) break;

                if (!occupied.Contains(nextPos))
                {
                    Room newRoom = new Room { id = roomIdCounter++, gridPos = nextPos };
                    roomDict[nextPos] = newRoom;
                    rooms.Add(newRoom);
                    occupied.Add(nextPos);

                    roomDict[current].connectedRooms.Add(nextPos);
                    newRoom.connectedRooms.Add(current);

                    toExplore.Enqueue(nextPos);
                }
                else
                {
                    if (!roomDict[current].connectedRooms.Contains(nextPos))
                    {
                        roomDict[current].connectedRooms.Add(nextPos);
                        roomDict[nextPos].connectedRooms.Add(current);
                    }
                }
            }
        }

        // 방 & 복도 그리기
        foreach (var room in rooms)
        {
            Vector2Int worldPos = room.gridPos * new Vector2Int(roomSpacing, roomSpacing);
            DrawRoomFloor(worldPos, roomWidth, roomHeight, room);

            foreach (var conn in room.connectedRooms)
            {
                Vector2Int connWorldPos = conn * new Vector2Int(roomSpacing, roomSpacing);
                DrawCorridor(worldPos, connWorldPos);
            }
        }

        groundTiles = groundTiles.Distinct().ToList();

        // 계단 배치
        PlaceStairs();

        // 아이템 생성
        SpawnItems();

        // 벽 생성
        GenerateWalls();
    }

    void DrawRoomFloor(Vector2Int worldPos, int width, int height, Room room)
    {
        int startX = worldPos.x - width / 2;
        int startY = worldPos.y - height / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePos = new Vector3Int(startX + x, startY + y, 0);

                TileBase selectedGroundTile = GetRandomGroundTile();
                groundTilemap.SetTile(tilePos, selectedGroundTile);
                groundTiles.Add(tilePos);
                room.tiles.Add(tilePos);
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

                TileBase selectedGroundTile = GetRandomGroundTile();
                groundTilemap.SetTile(tilePos, selectedGroundTile);
                groundTiles.Add(tilePos);
            }
            pos += dir;
        }
    }

    // 확률 기반 타일 선택 함수
    TileBase GetRandomGroundTile()
    {

        // 일반 스테이지
        float[] tileRates = new float[groundTilesByStage.Length];
        tileRates[0] = 1f; // 기본 타일 확률 100%에서 시작

        // 두 번째 타일부터 확률 계산
        for (int i = 1; i < groundTilesByStage.Length; i++)
        {
            // 스테이지별 확률 계산 (5% ~ 30%) -> 생기는 디테일 조정 필요
            // 희망 사항: 5%에서 30%까지 선형적으로 증가 & 보스 스테이지 단위로 새로운 타일 등장
            float rate = Mathf.Lerp(0.05f, 0.3f, (float)currentStage / (maxStage - specialStageCount - 1));
            tileRates[i] = rate;
            tileRates[0] -= rate; // 기본 타일 확률에서 차감
        }

        // 랜덤 선택
        float rand = Random.value;
        float acc = 0f;
        for (int i = 0; i < groundTilesByStage.Length; i++)
        {
            acc += tileRates[i];
            if (rand < acc)
                return groundTilesByStage[i];
        }
        return groundTilesByStage[0];
    }

    // 스테이지 변경 시 currentStage 값을 바꿔주고 GenerateMap() 호출
    public void NextStage()
    {
        currentStage++;
        StageManager.CurrentStage = currentStage;

        Debug.Log("Current Stage: " + currentStage);

        if (currentStage == 5 || currentStage == 10 || currentStage == 15)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("BossMap");
            Debug.Log("Boss Stage: " + currentStage);
            return;
        }

        GenerateMap();
    }

    void PlaceStairs()
    {
        if (rooms.Count < 2) return;

        var shuffledRooms = rooms.OrderBy(r => Random.value).ToList();
        Room upRoom = shuffledRooms[0];
        Room downRoom = shuffledRooms[1];

        var upRoomTiles = upRoom.tiles.Where(t => IsInsideRoom(t)).ToList();
        var downRoomTiles = downRoom.tiles.Where(t => IsInsideRoom(t)).ToList();

        if (upRoomTiles.Count == 0 || downRoomTiles.Count == 0) return;

        Vector3Int upPos = upRoomTiles[Random.Range(0, upRoomTiles.Count)];
        Vector3Int downPos = downRoomTiles[Random.Range(0, downRoomTiles.Count)];

        PlaceStairArea(upPos, stairUpTile);
        PlaceStairArea(downPos, stairDownTile);
    }

    bool IsInsideRoom(Vector3Int tilePos)
    {
        Vector3Int[] dirs =
        {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

        int tileCount = Mathf.Clamp(currentStage + 1, 1, groundTilesByStage.Length);
        TileBase[] candidates = groundTilesByStage.Take(tileCount).ToArray();

        foreach (var dir in dirs)
        {
            TileBase tile = groundTilemap.GetTile(tilePos + dir);
            if (!candidates.Contains(tile))
                return false;
        }
        return true;
    }

    void PlaceStairArea(Vector3Int centerPos, TileBase stairTile)
    {
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Vector3Int pos = new Vector3Int(centerPos.x + x, centerPos.y + y, 0);
                groundTilemap.SetTile(pos, stairTile);
            }
        }
    }

    void SpawnItems()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0) return;

        int spawnCount = Mathf.Min(maxItemCount, groundTiles.Count);
        var shuffled = groundTiles.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3Int tilePos = shuffled[i];
            Vector3 worldPos = groundTilemap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);

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
