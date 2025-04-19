using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[RequireComponent(typeof(Tilemap))]
public class WorldGen : MonoBehaviour
{
    [Header("Tile References")]
    public Tile walkableTile;
    public Tile obstacleTile;

    [Header("Map Settings")]
    public int width = 100;
    public int height = 100;
    public bool generateOnStart = true;

    [Header("Cave Generation Settings")]
    [Range(0, 1)] public float randomFillPercent = 0.45f;
    public int smoothIterations = 5;
    public int pitThresholdSize = 50;
    public int roomThresholdSize = 50;
    public int passageRadius = 1;

    private Tilemap tilemap;
    private bool[,] caveMap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        if(generateOnStart)
            GenerateCave();
    }

    public void GenerateCave()
    {
        InitializeMap();

        for(int i = 0; i < smoothIterations; i++)
            SmoothMap();

        ProcessMap();
        RenderMap();
    }

    void InitializeMap()
    {
        caveMap = new bool[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    caveMap[x, y] = false;
                else
                    caveMap[x, y] = Random.value > randomFillPercent;
            }
        }
    }

    void SmoothMap()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                int neighborPitTiles = GetSurroundingPitCount(x, y);

                if(neighborPitTiles > 4)
                    caveMap[x, y] = false;
                else if(neighborPitTiles < 4)
                    caveMap[x, y] = true;
            }
        }
    }

    int GetSurroundingPitCount(int gridX, int gridY)
    {
        int pitCount = 0;
        for(int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for(int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if(neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if(neighborX != gridX || neighborY != gridY)
                        pitCount += caveMap[neighborX, neighborY] ? 0 : 1;
                }
                else
                    pitCount++;
            }
        }
        return pitCount;
    }

    void ProcessMap()
    {
        List<List<Vector2Int>> pitRegions = GetRegions(false);
        foreach(List<Vector2Int> region in pitRegions)
        {
            if(region.Count < pitThresholdSize)
            {
                foreach(Vector2Int tile in region)
                    caveMap[tile.x, tile.y] = true;
            }
        }

        List<List<Vector2Int>> roomRegions = GetRegions(true);
        List<Room> survivingRooms = new List<Room>();
        foreach(List<Vector2Int> region in roomRegions)
        {
            if(region.Count < roomThresholdSize)
            {
                foreach (Vector2Int tile in region)
                    caveMap[tile.x, tile.y] = false;
            }
            else
                survivingRooms.Add(new Room(region, caveMap));
        }

        if(survivingRooms.Count > 0)
            ConnectClosestRooms(survivingRooms);
    }

    void ConnectClosestRooms(List<Room> allRooms)
    {
        foreach(Room room in allRooms)
        {
            if(room.connectedRooms.Count > 0)
                continue;
            Room closestRoom = null;
            float closestDistance = float.MaxValue;
            Vector2Int bestTileA = new Vector2Int();
            Vector2Int bestTileB = new Vector2Int();
            foreach(Room otherRoom in allRooms)
            {
                if(room == otherRoom || room.IsConnected(otherRoom))
                    continue;
                for(int i = 0; i < room.edgeTiles.Count; i++)
                {
                    for(int j = 0; j < otherRoom.edgeTiles.Count; j++)
                    {
                        Vector2Int tileA = room.edgeTiles[i];
                        Vector2Int tileB = otherRoom.edgeTiles[j];
                        float distance = Vector2Int.Distance(tileA, tileB);
                        if(distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestRoom = otherRoom;
                            bestTileA = tileA;
                            bestTileB = tileB;
                        }
                    }
                }
            }

            if(closestRoom != null)
                CreatePassage(room, closestRoom, bestTileA, bestTileB);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Vector2Int tileA, Vector2Int tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        List<Vector2Int> line = GetLine(tileA, tileB);
        foreach(Vector2Int point in line)
            DrawCircle(point, passageRadius);
    }

    void DrawCircle(Vector2Int point, int radius)
    {
        for(int x = -radius; x <= radius; x++)
        {
            for(int y = -radius; y <= radius; y++)
            {
                if(x * x + y * y <= radius * radius)
                {
                    int drawX = point.x + x;
                    int drawY = point.y + y;
                    if(IsInMapRange(drawX, drawY))
                        caveMap[drawX, drawY] = true;
                }
            }
        }
    }

    List<Vector2Int> GetLine(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> line = new List<Vector2Int>();

        int x = from.x;
        int y = from.y;
        int dx = to.x - from.x;
        int dy = to.y - from.y;

        bool inverted = false;
        int step = (int)Mathf.Sign(dx);
        int gradientStep = (int)Mathf.Sign(dy);
        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);
        if(longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = (int)Mathf.Sign(dy);
            gradientStep = (int)Mathf.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for(int i = 0; i < longest; i++)
        {
            line.Add(new Vector2Int(x, y));

            if(inverted)
                y += step;
            else
                x += step;

            gradientAccumulation += shortest;
            if(gradientAccumulation >= longest)
            {
                if(inverted)
                    x += gradientStep;
                else
                    y += gradientStep;
                gradientAccumulation -= longest;
            }
        }
        return line;
    }

    List<List<Vector2Int>> GetRegions(bool tileType)
    {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        bool[,] mapFlags = new bool[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(!mapFlags[x, y] && caveMap[x, y] == tileType)
                {
                    List<Vector2Int> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);
                    foreach (Vector2Int tile in newRegion)
                        mapFlags[tile.x, tile.y] = true;
                }
            }
        }
        return regions;
    }

    List<Vector2Int> GetRegionTiles(int startX, int startY)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        bool[,] mapFlags = new bool[width, height];
        bool tileType = caveMap[startX, startY];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        mapFlags[startX, startY] = true;
        while(queue.Count > 0)
        {
            Vector2Int tile = queue.Dequeue();
            tiles.Add(tile);
            for(int x = tile.x - 1; x <= tile.x + 1; x++)
            {
                for(int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    if(IsInMapRange(x, y) && (y == tile.y || x == tile.x))
                    {
                        if(!mapFlags[x, y] && caveMap[x, y] == tileType)
                        {
                            mapFlags[x, y] = true;
                            queue.Enqueue(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void RenderMap()
    {
        tilemap.ClearAllTiles();
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                tilemap.SetTile(position, caveMap[x, y] ? walkableTile : obstacleTile);
            }
        }
    }

    class Room
    {
        public List<Vector2Int> tiles;
        public List<Vector2Int> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;

        public Room() { }

        public Room(List<Vector2Int> roomTiles, bool[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Vector2Int>();
            foreach(Vector2Int tile in tiles)
            {
                for(int x = tile.x - 1; x <= tile.x + 1; x++)
                {
                    for(int y = tile.y - 1; y <= tile.y + 1; y++)
                    {
                        if(x == tile.x || y == tile.y)
                        {
                            if(x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1))
                            {
                                if(map[x, y] == false)
                                    edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }
    }
}