using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("Tilemap References")]
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    //public TileBase groundTile;
    // 기존 groundTile 대신 배열로 선언
    public TileBase[] groundTilesByStage;
    public TileBase wallTileHorizontal;
    public TileBase wallTileVertical;

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

    public GameObject shopPrefab; // 인스펙터에서 상점 프리팹 할당
    private GameObject shopInstance; // 현재 맵에 생성된 상점 오브젝트 참조

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

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        // 스테이지 1부터 시작
        currentStage = StageManager.CurrentStage;
        if (currentStage < 1) currentStage = 1;
        GenerateMap();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {

            //GenerateMap();
            /*            NextStage(true);
                        Debug.Log("Map regenerated");*/
            NextStage(true);
            MapUIManager ui = Object.FindFirstObjectByType<MapUIManager>();
            if (ui != null)
            {
                ui.OnStageEnd();
            }
        }
    }

    void GenerateMap()
    {
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        groundTiles.Clear();
        rooms.Clear();

        // 4, 9, 14번째 스테이지는 구역 8개
        if (currentStage == 4 || currentStage == 9 || currentStage == 14)
        {
            roomCount = 8;
        }
        else
        {
            roomCount = 7;
        }

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

        // 아이템 생성
        SpawnItems();

        // 벽 생성
        GenerateWalls();

        // 계단 배치
        PlaceStairs();

        // 상점 프리팹 배치 (4, 9, 14번째 스테이지)
        if (shopPrefab != null && (currentStage == 4 || currentStage == 9 || currentStage == 14))
        {
            shopInstance = PlaceShop();
        }
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
        // 기본 타일 외 타일의 등장 확률 (1~5%)
        float minRate = 0.01f;
        float maxRate = 0.05f;
        float rate = Mathf.Lerp(minRate, maxRate, Mathf.InverseLerp(2, 14, currentStage));

        // 스테이지별 등장 타일 인덱스 결정
        int extraStart = 2;
        int extraEnd = 1; // 기본값: 첫번째 스테이지는 추가 타일 없음

        if (currentStage >= 2 && currentStage <= 3)
            extraEnd = 2;
        else if (currentStage >= 4 && currentStage <= 5)
            extraEnd = 3;
        else if (currentStage >= 6 && currentStage <= 7)
            extraEnd = 4;
        else if (currentStage >= 8 && currentStage <= 9)
            extraEnd = 5;
        else if (currentStage >= 10 && currentStage <= 11)
            extraEnd = 6;
        else if (currentStage >= 12 && currentStage <= 13)
            extraEnd = 7;
        else if (currentStage == 14)
            extraEnd = 8;
        else if (currentStage == 15)
            extraEnd = 9;

        // 등장 타일 인덱스 리스트 생성
        List<int> extraTileIndices = new List<int>();
        for (int i = extraStart; i <= extraEnd && i < groundTilesByStage.Length; i++)
            extraTileIndices.Add(i);

        // 확률 분배
        float defaultTileRate = 1f - (rate * extraTileIndices.Count);
        List<float> tileRates = new List<float>();
        tileRates.Add(defaultTileRate); // 0번 타일

        for (int i = 1; i < groundTilesByStage.Length; i++)
        {
            if (extraTileIndices.Contains(i))
                tileRates.Add(rate);
            else
                tileRates.Add(0f);
        }

        // 랜덤 선택
        float rand = Random.value;
        float acc = 0f;
        for (int i = 0; i < tileRates.Count; i++)
        {
            acc += tileRates[i];
            if (rand < acc)
                return groundTilesByStage[i];
        }
        return groundTilesByStage[0];
    }

    // 스테이지 변경 시 currentStage 값을 바꿔주고 GenerateMap() 호출
    public void NextStage(bool isStairUp)
    {
        ClearItems();

        StageManager.AdvanceStage(isStairUp);
        currentStage = StageManager.CurrentStage;

        /*        Debug.Log("Current Stage: " + currentStage);

                if (currentStage == 5 || currentStage == 10 || currentStage == 15)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("BossMap");
                    Debug.Log("Boss Stage: " + currentStage);

                    Debug.Log("Ready for Boss Stage: " + currentStage);
                    return;
                }

                GenerateMap();*/

        if (StageManager.IsBossStage())
        {
            string bossScene = StageManager.GetBossScene();
            Debug.Log("Boss Stage: " + currentStage + " -> Loading: " + bossScene);
            SceneManager.LoadScene(bossScene);
            return;
        }

        // 일반 맵
        string mapScene = StageManager.GetMapScene();
        Debug.Log("Map Stage: " + currentStage + " -> Loading: " + mapScene);
        GenerateMap(); // 맵 재생성
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

        // 상점이 있는 방의 타일 제외
        List<Vector3Int> shopRoomTiles = new List<Vector3Int>();
        if (shopInstance != null)
        {
            // shopInstance가 있는 방 찾기
            Room shopRoom = rooms.FirstOrDefault(r =>
                r.tiles.Any(t =>
                {
                    Vector3 worldPos = groundTilemap.CellToWorld(t) + new Vector3(0.5f, 0.5f, 0);
                    return Vector3.Distance(worldPos, shopInstance.transform.position) < 1f;
                })
            );

            if (shopRoom != null)
                shopRoomTiles = shopRoom.tiles;
        }

        // 방 내부 타일만 대상으로, 상점 타일 제외
        var roomTiles = rooms.SelectMany(r => r.tiles)
                             .Where(t => !shopRoomTiles.Contains(t))
                             .Distinct()
                             .ToList();

        // 계단 타일 제외
        var validTiles = roomTiles.Where(tilePos =>
        {
            TileBase tile = groundTilemap.GetTile(tilePos);
            return tile != stairUpTile && tile != stairDownTile;
        }).ToList();

        int spawnCount = Mathf.Min(maxItemCount, validTiles.Count);
        var shuffled = validTiles.OrderBy(x => Random.value).ToList();

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
        //MapUIManager.Instance.OnStageStart();

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

                if (groundTilemap.GetTile(pos) == null) // 땅이 아니면 벽 후보
                {
                    if (HasGroundNeighbour(pos))
                    {
                        bool hasHorizontalNeighbour =
                            groundTilemap.GetTile(new Vector3Int(pos.x - 1, pos.y, 0)) != null ||
                            groundTilemap.GetTile(new Vector3Int(pos.x + 1, pos.y, 0)) != null;

                        bool hasVerticalNeighbour =
                            groundTilemap.GetTile(new Vector3Int(pos.x, pos.y - 1, 0)) != null ||
                            groundTilemap.GetTile(new Vector3Int(pos.x, pos.y + 1, 0)) != null;

                        if (hasHorizontalNeighbour && !hasVerticalNeighbour)
                            wallTilemap.SetTile(pos, wallTileHorizontal);
                        else if (hasVerticalNeighbour && !hasHorizontalNeighbour)
                            wallTilemap.SetTile(pos, wallTileVertical);
                        else
                            wallTilemap.SetTile(pos, wallTileVertical); // 코너도 vertical로 설정
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

    // 상점 프리팹을 8번째 방 중앙에 배치
    GameObject PlaceShop()
    {
        if (rooms.Count < 8) return null;
        Room shopRoom = rooms[7]; // 8번째 방 (인덱스 7)
        // 방 중앙 좌표 계산
        Vector3 avgWorldPos = Vector3.zero;
        foreach (var tile in shopRoom.tiles)
        {
            avgWorldPos += groundTilemap.CellToWorld(tile) + new Vector3(0.5f, 0.5f, 0);
        }
        avgWorldPos /= shopRoom.tiles.Count;

        return Instantiate(shopPrefab, avgWorldPos, Quaternion.identity, this.transform);
    }
}
