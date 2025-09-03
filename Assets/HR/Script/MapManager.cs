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

    [Header("Stage Settings")] public int currentStage = 0; //              

    [Header("Room Settings")] public int roomCount = 7;
    public int roomWidth = 13;
    public int roomHeight = 13;

    [Header("Spacing Settings")] public int roomSpacing = 25;

    [Header("Corridor Settings")] public int corridorWidth = 3;

    [Header("Stair Tiles")] public TileBase stairUpTile;
    public TileBase stairDownTile;

    [Header("Item Spawn")] public GameObject[] itemPrefabs;
    public int maxItemCount = 10;

    public GameObject shopPrefab; //  ν    Ϳ                 Ҵ 
    private GameObject shopInstance; //       ʿ                    Ʈ     

    //      ٴ  Ÿ  
    private List<Vector3Int> groundTiles = new List<Vector3Int>();

    const int maxStage = 15; //   ü            
    const int specialStageCount = 2; // Ư              

    private List<GameObject> roomGameObjects = new List<GameObject>();

    //               
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
        currentStage = StageManager.CurrentStage;
        if (currentStage < 1) currentStage = 1;
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
        foreach (Room room in rooms)
        {
            Destroy(room.roomObject);
        }

        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        groundTiles.Clear();
        rooms.Clear();

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

        int roomIdCounter = 0;

        Room startRoom = RoomObject(startPos, roomIdCounter++);
        Destroy(startRoom.roomObject.GetComponent<EnemySpawner>());
        roomDict[startPos] = startRoom;
        rooms.Add(startRoom);
        occupied.Add(startPos);


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

        if (shopPrefab != null && (currentStage == 4 || currentStage == 9 || currentStage == 14))
        {
            shopInstance = PlaceShop();
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
        float minRate = 0.01f;
        float maxRate = 0.05f;
        float rate = Mathf.Lerp(minRate, maxRate, Mathf.InverseLerp(2, 14, currentStage));

        int extraStart = 2;
        int extraEnd = 1; //  ⺻  : ù  °             ߰  Ÿ       

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

        List<int> extraTileIndices = new List<int>();
        for (int i = extraStart; i <= extraEnd && i < groundTilesByStage.Length; i++)
            extraTileIndices.Add(i);

        float defaultTileRate = 1f - (rate * extraTileIndices.Count);
        List<float> tileRates = new List<float>();
        tileRates.Add(defaultTileRate); // 0   Ÿ  

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

    public async Task NextStage(bool isStairUp)
    {
        ClearItems();

        StageManager.AdvanceStage(isStairUp);
        currentStage = StageManager.CurrentStage;


        if (StageManager.IsBossStage())
        {
            string bossScene = StageManager.GetBossScene();
            Debug.Log("Boss Stage: " + currentStage + " -> Loading: " + bossScene);
            SceneManager.LoadScene(bossScene);
            return;
        }

        //  Ϲ    
        string mapScene = StageManager.GetMapScene();
        Debug.Log("Map Stage: " + currentStage + " -> Loading: " + mapScene);
        GenerateMap(); //         
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

        //         ִ       Ÿ       
        List<Vector3Int> shopRoomTiles = new List<Vector3Int>();
        if (shopInstance != null)
        {
            // shopInstance    ִ     ã  
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

        //         Ÿ ϸ         ,      Ÿ       
        var roomTiles = rooms.SelectMany(r => r.tiles)
            .Where(t => !shopRoomTiles.Contains(t))
            .Distinct()
            .ToList();

        //     Ÿ       
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
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Item"))
            {
                Destroy(child.gameObject);
            }
        }
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
        if (rooms.Count < 8) return null;
        Room shopRoom = rooms[7]; // 8  °    ( ε    7)
        //     ߾    ǥ    
        Vector3 avgWorldPos = Vector3.zero;
        foreach (var tile in shopRoom.tiles)
        {
            avgWorldPos += groundTilemap.CellToWorld(tile) + new Vector3(0.5f, 0.5f, 0);
        }

        avgWorldPos /= shopRoom.tiles.Count;

        return Instantiate(shopPrefab, avgWorldPos, Quaternion.identity, this.transform);
    }
}