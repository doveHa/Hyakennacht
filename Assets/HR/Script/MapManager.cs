using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Enemy;
using Manager;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("Tilemap References")] public Tilemap groundTilemap;
    public Tilemap wallTilemap;

    public TileBase[] groundTilesByStage;
    public TileBase wallTileHorizontal;
    public TileBase wallTileVertical;

    //[Header("Stage Settings")] public int currentStage = 0;

    [Header("Room Settings")] public int roomCount = 7;
    public int roomWidth = 13;
    public int roomHeight = 13;

    [Header("Spacing Settings")] public int roomSpacing = 25;

    [Header("Corridor Settings")] public int corridorWidth = 3;

    [Header("Stair Tiles")] public TileBase stairUpTile;
    public TileBase stairDownTile;

    [Header("Item Spawn")] public GameObject[] itemPrefabs;
    public int maxItemCount = 10;

    [Header("Dynamic Settings")]
    [SerializeField] private int largeRoomCount = 8; // 방 개수가 많은 스테이지의 방 수
    [SerializeField] private int normalRoomCount = 7; // 일반 스테이지의 방 수
    [SerializeField] private int shopRoomIndex = 6;  // 상점 방이 생성될 rooms 리스트 인덱스

    //[Header("Tile Settings")]
    //[SerializeField] private int minTileStage = 2; // 타일 확률 계산 시작 스테이지
    //[SerializeField] private int maxTileStage = 14; // 타일 확률 계산 끝 스테이지

    [System.Serializable]
    public class TileStageData
    {
        public int endStage; // 해당 데이터가 적용되는 마지막 스테이지
        public int extraTileIndexEnd; // 추가 타일 인덱스 범위의 마지막
        public float tileRate; // 추가 타일이 나올 확률
    }

    [Header("Tile Settings")]
    public List<TileStageData> tileDataByStage;

    public GameObject shopPrefab;
    private GameObject shopInstance;

    private List<Vector3Int> groundTiles = new List<Vector3Int>();

    private Room _startRoom;

    [System.Serializable]
    public class Room
    {
        public Room(int id, Vector2Int gridPos, GameObject roomObject)
        {
            this.id = id;
            this.gridPos = gridPos;
            this.roomObject = roomObject;
            tilemap = roomObject.GetComponent<Tilemap>();
        }

        public int id;
        public Vector2Int gridPos; //      ġ   ǥ
        public List<Vector3Int> tiles = new List<Vector3Int>(); //         Ÿ  
        public List<Vector2Int> connectedRooms = new List<Vector2Int>();
        public GameObject roomObject;
        public Tilemap tilemap;
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
        GenerateMap();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NextStage(true);
            MapUIManager ui = Object.FindFirstObjectByType<MapUIManager>();
            if (ui != null)
            {
                //ui.OnStageEnd();
            }
        }
    }

    void GenerateMap()
    {
        // StageManager의 보스 스테이지 정보를 사용하여 방 개수 설정
        if (StageManager.IsBossStage())
        {
            roomCount = largeRoomCount;
        }
        else
        {
            roomCount = normalRoomCount;
        }

        Dictionary<Vector2Int, Room> roomDict = new Dictionary<Vector2Int, Room>();
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        Vector2Int startPos = Vector2Int.zero;
        Queue<Vector2Int> toExplore = new Queue<Vector2Int>();
        toExplore.Enqueue(startPos);

        int roomIdCounter = 0;

        _startRoom = RoomObject(startPos, roomIdCounter++);
        Destroy(_startRoom.roomObject.GetComponent<EnemySpawner>());
        roomDict[startPos] = _startRoom;
        rooms.Add(_startRoom);
        occupied.Add(startPos);

        GameManager.Manager.Player.transform.position = _startRoom.roomObject.transform.position;

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
                    Room newRoom = RoomObject(nextPos, roomIdCounter++);
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

        foreach (var room in rooms)
        {
            Vector2Int worldPos = room.gridPos * new Vector2Int(roomSpacing, roomSpacing);
            DrawRoomFloor(worldPos, roomWidth, roomHeight, room);

            foreach (var conn in room.connectedRooms)
            {
                Vector2Int connWorldPos = conn * new Vector2Int(roomSpacing, roomSpacing);
                DrawCorridor(worldPos, connWorldPos, room);
            }
        }

        groundTiles = groundTiles.Distinct().ToList();

        SpawnItems();

        GenerateWalls(groundTilemap, wallTilemap);

        PlaceStairs();

        if (shopPrefab != null && StageManager.IsShopStage())
        {
            shopInstance = PlaceShop();
        }

        // 새로운 씬이 로드된 후 MapUIManager의 OnStageStart()를 호출
        if (MapUIManager.Instance != null)
        {
            MapUIManager.Instance.OnStageStart();
        }
    }

    private Room RoomObject(Vector2Int pos, int roomIdCounter)
    {
        GameObject roomObject = new GameObject();
        roomObject.transform.parent = GameObject.Find("Grid").transform;
        roomObject.AddComponent<Tilemap>();
        roomObject.AddComponent<TilemapRenderer>().sortingOrder = -4;
        roomObject.AddComponent<EnemySpawner>();

        Room room = new Room(roomIdCounter, pos, roomObject);
        return room;
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
                room.tilemap.SetTile(tilePos, selectedGroundTile);
                groundTiles.Add(tilePos);
                room.tiles.Add(tilePos);
            }
        }
    }

    void DrawCorridor(Vector2Int from, Vector2Int to, Room room)
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

    TileBase GetRandomGroundTile()
    {
        float rate = 0f;
        int extraEnd = 1;

        // 현재 스테이지에 맞는 타일 데이터를 찾습니다.
        foreach (var stageData in tileDataByStage)
        {
            if (StageManager.CurrentStage <= stageData.endStage)
            {
                rate = stageData.tileRate;
                extraEnd = stageData.extraTileIndexEnd;
                break; // 데이터를 찾았으니 루프를 빠져나갑니다.
            }
        }

        // 타일 데이터가 설정되지 않았을 경우를 위한 안전장치
        if (rate == 0f)
        {
            rate = 0.01f;
            extraEnd = 1;
        }

        List<int> extraTileIndices = new List<int>();
        for (int i = 2; i <= extraEnd && i < groundTilesByStage.Length; i++)
            extraTileIndices.Add(i);

        float defaultTileRate = 1f - (rate * extraTileIndices.Count);
        List<float> tileRates = new List<float>();
        tileRates.Add(defaultTileRate); // 기본 타일 확률

        for (int i = 1; i < groundTilesByStage.Length; i++)
        {
            if (extraTileIndices.Contains(i))
                tileRates.Add(rate);
            else
                tileRates.Add(0f);
        }

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

    public static void NextStage(bool isStairUp)
    {
        StageManager.AdvanceStage(isStairUp);
        if (StageManager.IsBossStage())
        {
            string bossScene = StageManager.GetBossScene();
            SceneManager.LoadScene(bossScene);
            return;
        }

        SceneManager.LoadScene(StageManager.GetMapScene());
    }

    void PlaceStairs()
    {
        if (rooms.Count < 2) return;

        var shuffledRooms = rooms.OrderBy(r => Random.value).ToList();
        var availableRooms = shuffledRooms.Where(r => r != _startRoom).ToList();
        Room upRoom = availableRooms[0];
        Room downRoom = availableRooms[1];

        var upRoomTiles = upRoom.tiles.Where(t => IsInsideRoom(t)).ToList();
        var downRoomTiles = downRoom.tiles.Where(t => IsInsideRoom(t)).ToList();

        if (upRoomTiles.Count == 0 || downRoomTiles.Count == 0) return;

        Vector3Int upPos = upRoomTiles[Random.Range(1, upRoomTiles.Count - 1)];
        Vector3Int downPos = downRoomTiles[Random.Range(1, downRoomTiles.Count - 1)];

        Vector3 offset = new Vector3(0.5f, 0.5f, 0);
        Instantiate(AddressableManager.Manager.GetPrefabByName("UpStair"), upPos + offset, Quaternion.identity)
            .transform.parent = upRoom.roomObject.transform;
        Instantiate(AddressableManager.Manager.GetPrefabByName("DownStair"), downPos + offset, Quaternion.identity)
            .transform.parent = downRoom.roomObject.transform;
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

        int tileCount = Mathf.Clamp(StageManager.CurrentStage + 1, 1, groundTilesByStage.Length);
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

        List<Vector3Int> shopRoomTiles = new List<Vector3Int>();
        if (shopInstance != null)
        {
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

        // 방 타일과 복도 타일을 명확히 구분하여 처리하는 새로운 방법
        List<Vector3Int> roomOnlyTiles = new List<Vector3Int>();
        foreach (var room in rooms)
        {
            // 각 방 타일들을 순회하며 복도와 인접하지 않은 타일만 추가
            foreach (var tile in room.tiles)
            {
                // 타일 주변 8방향 중 복도 타일이 없는지 확인
                if (!IsCorridorOrEntranceNeighbour(tile))
                {
                    roomOnlyTiles.Add(tile);
                }
            }
        }

        // 계단 타일이 아니고, 상점 타일도 아니며, 복도와 인접하지 않은 타일만 선택
        var validTiles = roomOnlyTiles.Where(tilePos =>
        {
            TileBase tile = groundTilemap.GetTile(tilePos);
            // 계단 타일과 상점 방에 있는 타일을 제외
            return tile != stairUpTile && tile != stairDownTile && !shopRoomTiles.Contains(tilePos);
        }).ToList();

        int spawnCount = Mathf.Min(maxItemCount, validTiles.Count);
        var shuffled = validTiles.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < spawnCount; i++)
        {
            if (i >= shuffled.Count) break; // 혹시 모를 예외 방지
            Vector3Int tilePos = shuffled[i];
            Vector3 worldPos = groundTilemap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);

            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject selectedPrefab = itemPrefabs[randomIndex];

            Instantiate(selectedPrefab, worldPos, Quaternion.identity, this.transform);
        }
    }

    // 타일이 복도나 방 입구와 인접한지 확인하는 함수
    private bool IsCorridorOrEntranceNeighbour(Vector3Int tilePos)
    {
        // 복도 타일의 기준:
        // 1. 방 타일 리스트에는 없지만 groundTiles 리스트에는 있는 타일
        // 2. 여러 방에 속한 타일 (이 경우는 구현이 더 복잡하므로 단순화)

        // 복도와 방 입구는 겹치는 지점이므로, 간단하게 방 외부에 있는 바닥 타일과 인접한 경우를 확인

        Vector3Int[] directions =
        {
            new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
            new Vector3Int(1, 1, 0), new Vector3Int(1, -1, 0),
            new Vector3Int(-1, 1, 0), new Vector3Int(-1, -1, 0)
        };

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = tilePos + dir;

            // 현재 타일이 속한 방을 찾습니다.
            Room currentRoom = rooms.FirstOrDefault(r => r.tiles.Contains(tilePos));
            if (currentRoom == null) continue; // 방에 속하지 않는 타일이면 건너뜀

            // 이웃 타일이 현재 방에 속하지 않으면서도 바닥 타일인 경우
            if (!currentRoom.tiles.Contains(neighborPos) && groundTiles.Contains(neighborPos))
            {
                // 이웃 타일이 다른 방 타일인지 확인
                Room neighborRoom = rooms.FirstOrDefault(r => r.tiles.Contains(neighborPos));

                // 이웃 타일이 다른 방에 속하거나, 어느 방에도 속하지 않는 경우 (복도)
                if (neighborRoom != null && neighborRoom.id != currentRoom.id || neighborRoom == null)
                {
                    return true; // 복도와 인접한 타일이므로 아이템 생성 불가
                }
            }
        }

        return false; // 복도와 인접하지 않으므로 아이템 생성 가능
    }

    public void GenerateWalls(Tilemap flowTilemap, Tilemap generateWallTilemap, TileBase tileBase = null,
        bool isStageBlock = false)
    {
        BoundsInt bounds = flowTilemap.cellBounds;

        for (int x = bounds.xMin - 1; x <= bounds.xMax + 1; x++)
        {
            for (int y = bounds.yMin - 1; y <= bounds.yMax + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (flowTilemap.GetTile(pos) == null)
                {
                    if (HasGroundNeighbour(pos, flowTilemap))
                    {
                        if (isStageBlock)
                        {
                            generateWallTilemap.SetTile(pos, tileBase);
                        }
                        else
                        {
                            bool hasHorizontalNeighbour =
                                flowTilemap.GetTile(new Vector3Int(pos.x - 1, pos.y, 0)) != null ||
                                flowTilemap.GetTile(new Vector3Int(pos.x + 1, pos.y, 0)) != null;

                            bool hasVerticalNeighbour =
                                flowTilemap.GetTile(new Vector3Int(pos.x, pos.y - 1, 0)) != null ||
                                flowTilemap.GetTile(new Vector3Int(pos.x, pos.y + 1, 0)) != null;

                            if (hasHorizontalNeighbour && !hasVerticalNeighbour)
                                generateWallTilemap.SetTile(pos, wallTileHorizontal);
                            else if (hasVerticalNeighbour && !hasHorizontalNeighbour)
                                generateWallTilemap.SetTile(pos, wallTileVertical);
                            else
                                generateWallTilemap.SetTile(pos, wallTileVertical); //  ڳʵ  vertical       
                        }
                    }
                }
            }
        }
    }

    bool HasGroundNeighbour(Vector3Int pos, Tilemap tilemap)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                if (tilemap.GetTile(new Vector3Int(pos.x + dx, pos.y + dy, 0)) != null)
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

    GameObject PlaceShop()
    {
        if (shopRoomIndex < 0 || shopRoomIndex >= rooms.Count)
        {
            Debug.LogError($"Shop room index is out of range! Index: {shopRoomIndex}, Room Count: {rooms.Count}");
            return null;
        }

        Room shopRoom = rooms[shopRoomIndex];
        Vector3 avgWorldPos = Vector3.zero;
        foreach (var tile in shopRoom.tiles)
        {
            avgWorldPos += groundTilemap.CellToWorld(tile) + new Vector3(0.5f, 0.5f, 0);
        }

        avgWorldPos /= shopRoom.tiles.Count;

        return Instantiate(shopPrefab, avgWorldPos, Quaternion.identity, this.transform);
    }
}