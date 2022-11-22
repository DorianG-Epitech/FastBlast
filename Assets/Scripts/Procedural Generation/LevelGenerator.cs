using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public enum CellType
    {
        None,
        Room,
        Hallway,
        Stairs
    }

    public Vector3Int size;
    public int roomCount;
    public Vector3Int roomMaxSize;
    public float statsMultipleCorridor = 0.125f;

    private Random random;
    public Grid<CellType> grid;
    public List<Room> rooms;
    private Dictionary<Vector3Int, GameObject> _corridors;
    DelaunayTriangulation delaunay;
    HashSet<Prim.Edge> selectedEdges;

    public List<GameObject> prefabs;

    public GameObject corridorPrefab;
    public GameObject stairPrefab;
    public GameObject wallPrefab;

    public NavigationBaker baker;

    public int gridRealSizeX;
    public int gridRealSizeY;
    public int gridRealSizeZ;

    private List<Enemies.MultiSpawner> _spawnersList;

    private bool canProgressToNextLevel = false;

    public void GenerateLevel(int lvlDifficulty)
    {
        canProgressToNextLevel = false;
        DestroyMap();

        while (!canProgressToNextLevel) ;

        random = new Random();
        grid = new Grid<CellType>(size, Vector3Int.zero);
        rooms = new List<Room>();
        _corridors = new Dictionary<Vector3Int, GameObject>();
        _spawnersList = new List<Enemies.MultiSpawner>();

        SetupLevelDifficulty(lvlDifficulty);
        PlaceRooms();
        Triangulation();
        if (!CreateHallways()) {
            GenerateLevel(lvlDifficulty);
            return;
        }
        PathfindHallways();

        if (IsMapNotValid()) {
            GenerateLevel(lvlDifficulty);
            return;
        }

        baker.BakeNavMesh();
        setRoomSettings(lvlDifficulty);
    }

    public bool IsMapNotValid()
    {

        return false;
    }

    public void DestroyMap()
    {
        foreach (Room r in rooms)
        {
            Destroy(r.go);
        }

        if (_corridors != null)
        {
            foreach (Vector3Int key in _corridors.Keys)
            {
                Destroy(_corridors[key]);
            }
        }

        GameObject[] stairs = GameObject.FindGameObjectsWithTag("Stairs");
        foreach (GameObject s in stairs)
        {
            Destroy(s);
        }

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject w in walls)
        {
            Destroy(w);
        }

        // Destroy Ennemies
        GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in ennemies)
        {
            Destroy(e);
        }

        if (baker.surfaces == null)
            baker.surfaces = new List<NavMeshSurface>();
        
        baker.surfaces.Clear();
        canProgressToNextLevel = true;
    }

    public void SetupLevelDifficulty(int lvlDifficulty)
    {
        
    }

    private void PlaceRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            Vector3Int location = new Vector3Int(random.Next(0, size.x), random.Next(0, size.y), random.Next(0, size.z));


            int randomNum = random.Next(0, prefabs.Count);

            GameObject roomPrefab = prefabs[randomNum];

            bool add = true;

            Room newRoom = new Room(location, roomPrefab.GetComponent<Room>().size);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), newRoom.size + new Vector3Int(2, 0, 2));

            // check if rooms intersect
            foreach (var room in rooms)
            {
                if (Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            // check if room in the grid
            if (newRoom.bounds.xMin <= 0 || newRoom.bounds.xMax >= size.x || newRoom.bounds.yMin <= 0 || newRoom.bounds.yMax >= size.y || newRoom.bounds.zMin <= 0 || newRoom.bounds.zMax >= size.z)
               add = false;

            // add room to roomList
            if (add)
            {
                rooms.Add(newRoom.PlaceRoom(roomPrefab));

                Component[] nms = newRoom.go.GetComponents(typeof(NavMeshSurface));
                foreach (NavMeshSurface surf in nms)
                    baker.surfaces.Add(surf);

                foreach (var pos in newRoom.bounds.allPositionsWithin)
                    grid[pos] = CellType.Room;
            }
        }
    }

    private void Triangulation()
    {
        List<Vertex> vertices = new List<Vertex>();

        // Create Vertex Point in middle of each room of the grid
        foreach (var room in rooms)
            vertices.Add(new Vertex<Room>((Vector3)room.bounds.position + ((Vector3)room.bounds.size) / 2, room));

        delaunay = DelaunayTriangulation.Triangulate(vertices);
    }

    bool CreateHallways()
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges)
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        if (edges.Count == 0)
            return false;

        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);


        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges)
        {
            if (random.NextDouble() < statsMultipleCorridor)
            {
                selectedEdges.Add(edge);
            }
        }
        return true;
    }

    void PathfindHallways()
    {
        DungeonPathfinder aStar = new DungeonPathfinder(size);

        foreach (var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector3Int((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
            var endPos = new Vector3Int((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder.Node a, DungeonPathfinder.Node b) => {
                var pathCost = new DungeonPathfinder.PathCost();

                var delta = b.Position - a.Position;

                if (delta.y == 0)
                {
                    //flat hallway
                    pathCost.cost = Vector3Int.Distance(b.Position, endPos);    //heuristic

                    if (grid[b.Position] == CellType.Stairs)
                    {
                        return pathCost;
                    }
                    else if (grid[b.Position] == CellType.Room)
                    {
                        pathCost.cost += 5;
                    }
                    else if (grid[b.Position] == CellType.None)
                    {
                        pathCost.cost += 1;
                    }

                    pathCost.traversable = true;
                }
                else
                {
                    //staircase
                    if ((grid[a.Position] != CellType.None && grid[a.Position] != CellType.Hallway)
                        || (grid[b.Position] != CellType.None && grid[b.Position] != CellType.Hallway)) return pathCost;

                    pathCost.cost = 100 + Vector3Int.Distance(b.Position, endPos);    //base cost + heuristic

                    int xDir = Mathf.Clamp(delta.x, -1, 1);
                    int zDir = Mathf.Clamp(delta.z, -1, 1);
                    Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                    Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                    if (!grid.InBounds(a.Position + verticalOffset)
                        || !grid.InBounds(a.Position + horizontalOffset)
                        || !grid.InBounds(a.Position + verticalOffset + horizontalOffset))
                    {
                        return pathCost;
                    }

                    if (grid[a.Position + horizontalOffset] != CellType.None
                        || grid[a.Position + horizontalOffset * 2] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset * 2] != CellType.None)
                    {
                        return pathCost;
                    }

                    pathCost.traversable = true;
                    pathCost.isStairs = true;
                }

                return pathCost;
            });

            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];

                    if (grid[current] == CellType.None)
                    {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0)
                    {
                        var prev = path[i - 1];

                        var delta = current - prev;

                        if (delta.y != 0)
                        {
                            int xDir = Mathf.Clamp(delta.x, -1, 1);
                            int zDir = Mathf.Clamp(delta.z, -1, 1);
                            Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                            Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                            grid[prev + horizontalOffset] = CellType.Stairs;
                            grid[prev + horizontalOffset * 2] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset * 2] = CellType.Stairs;

                            Vector3Int stairsAdjustement = new Vector3Int(0, 0, 0);

                            int angle = 0;
                            if (xDir == -1) {
                                angle = 180;
                                if (delta.y == -1) {
                                    angle = 0;
                                } else {
                                    stairsAdjustement.x = -1;
                                }
                            } else if (xDir == 1) {
                                angle = 0;
                                if (delta.y == -1) {
                                    angle = 180;
                                } else {
                                    stairsAdjustement.x = 1;
                                }
                            } else if (zDir == -1) {
                                angle = 90;
                                if (delta.y == -1) {
                                    angle = 270;
                                } else {
                                    stairsAdjustement.z = -1;
                                }
                            } else if (zDir == 1) {
                                angle = 270;
                                if (delta.y == -1) {
                                    angle = 90;
                                } else {
                                    stairsAdjustement.z = 1;
                                }
                            }

                            if (delta.y == 1)
                                stairsAdjustement.y = 1;

                            PlaceStairs(prev + horizontalOffset + stairsAdjustement, angle);
                        }
                    }
                }

                foreach (var pos in path)
                {
                    if (grid[pos] == CellType.Hallway)
                    {
                        if (_corridors.ContainsKey(pos))
                        {
                            _corridors[pos] = setCorridorsWall(_corridors[pos], pos);
                        } else
                        {
                            PlaceHallway(pos);
                        }
                    }
                }
            }
        }
        for (int x = 0; x < size.x; x += 1)
        {
            for (int y = 0; y < size.y; y += 1)
            {
                for (int z = 0; z < size.z; z += 1)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    if (grid[pos] == CellType.Room)
                    {
                        setRoomWall(pos);
                    }
                }
            }
        }
    }

    private void setRoomWall(Vector3Int location)
    {
        
        Vector3Int checkNextPos = new Vector3Int(location.x + 1, location.y, location.z);
        if (grid[checkNextPos] == CellType.None)
        {
            Vector3Int realLocation = new Vector3Int((location.x * gridRealSizeX) + (gridRealSizeX / 2), location.y * gridRealSizeY, location.z * gridRealSizeZ);
            GameObject go = Instantiate(wallPrefab, realLocation, Quaternion.identity);
            go.GetComponent<Transform>().localScale = new Vector3Int(0, gridRealSizeY, gridRealSizeZ);
        }

        checkNextPos = new Vector3Int(location.x - 1, location.y, location.z);
        if (grid[checkNextPos] == CellType.None)
        {
            Vector3Int realLocation = new Vector3Int((location.x * gridRealSizeX) - (gridRealSizeX / 2), location.y * gridRealSizeY, location.z * gridRealSizeZ);
            GameObject go = Instantiate(wallPrefab, realLocation, Quaternion.identity);
            go.GetComponent<Transform>().localScale = new Vector3Int(0, gridRealSizeY, gridRealSizeZ);
        }

        checkNextPos = new Vector3Int(location.x, location.y, location.z + 1);
        if (grid[checkNextPos] == CellType.None)
        {
            Vector3Int realLocation = new Vector3Int(location.x * gridRealSizeX, location.y * gridRealSizeY, (location.z * gridRealSizeZ) + (gridRealSizeZ / 2));
            GameObject go = Instantiate(wallPrefab, realLocation, Quaternion.identity);
            go.GetComponent<Transform>().localScale = new Vector3Int(gridRealSizeX, gridRealSizeY, 0);
        }

        checkNextPos = new Vector3Int(location.x, location.y, location.z - 1);
        if (grid[checkNextPos] == CellType.None)
        {
            Vector3Int realLocation = new Vector3Int(location.x * gridRealSizeX, location.y * gridRealSizeY, (location.z * gridRealSizeZ) - (gridRealSizeZ / 2));
            GameObject go = Instantiate(wallPrefab, realLocation, Quaternion.identity);
            go.GetComponent<Transform>().localScale = new Vector3Int(gridRealSizeX, gridRealSizeY, 0);
        }
    }
    
    private GameObject setCorridorsWall(GameObject go, Vector3Int location)
    {
        Vector3Int checkNextPos = new Vector3Int(location.x + 1, location.y, location.z);
        if (location.x + 1 < size.x && grid[checkNextPos] != CellType.None)
        {
            GameObject t = go.transform.Find("WallXPlus").gameObject;
            if (t != null)
                t.SetActive(false);
        }

        checkNextPos = new Vector3Int(location.x - 1, location.y, location.z);
        if (location.x - 1 >= 0 && grid[checkNextPos] != CellType.None)
        {
            GameObject t = go.transform.Find("WallXMinus").gameObject;
            if (t != null)
                t.SetActive(false);
        }

        checkNextPos = new Vector3Int(location.x, location.y, location.z + 1);
        if (location.z + 1 < size.z && grid[checkNextPos] != CellType.None)
        {
            GameObject t = go.transform.Find("WallZPlus").gameObject;
            if (t != null)
                t.SetActive(false);
        }

        checkNextPos = new Vector3Int(location.x, location.y, location.z - 1);
        if (location.z - 1 >= 0 && grid[checkNextPos] != CellType.None)
        {
            GameObject t = go.transform.Find("WallZMinus").gameObject;
            if (t != null)
                t.SetActive(false);
        }

        return go;
    }

    void PlaceHallway(Vector3Int location)
    {
        Vector3Int realLocation = new Vector3Int(location.x * gridRealSizeX, location.y * gridRealSizeY, location.z * gridRealSizeZ);
        GameObject go = Instantiate(corridorPrefab, realLocation, Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3Int(gridRealSizeX, gridRealSizeY, gridRealSizeZ);

        go = setCorridorsWall(go, location);

        _corridors.Add(location, go);
    }

    void PlaceStairs(Vector3Int location, int angle)
    {
        Vector3Int realLocation = new Vector3Int(location.x * gridRealSizeX, location.y * gridRealSizeY, location.z * gridRealSizeZ);
        GameObject go = Instantiate(stairPrefab, realLocation, Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3Int(gridRealSizeX, gridRealSizeY, gridRealSizeZ);

        go.transform.Rotate(new Vector3(go.transform.eulerAngles.x, go.transform.eulerAngles.y + angle, go.transform.eulerAngles.z));
    }

    

    private void setRoomSettings(int lvlDifficulty)
    {
        int randomNum = random.Next(0, rooms.Count - 1);
        rooms[randomNum].isSpawn = true;
        rooms[randomNum].gameObject.transform.Find("MultiSpawner").gameObject.SetActive(false);


        while (rooms[randomNum].isEnd != true)
        {
            randomNum = random.Next(0, rooms.Count - 1);
            if (rooms[randomNum].isSpawn == false)
            {
                rooms[randomNum].isEnd = true;
                GameObject t = rooms[randomNum].gameObject.transform.Find("EndLevel").gameObject;
                if (t != null)
                    t.SetActive(true);
            }    
        }

        foreach(Room r in rooms)
        {
            if (r.isSpawn == false)
            {
                Enemies.MultiSpawner s = r.gameObject.transform.Find("MultiSpawner").gameObject.GetComponent<Enemies.MultiSpawner>();
                _spawnersList.Add(s);
                s.SpawnEnnemy(random.Next(0, 5 * lvlDifficulty), random.Next(0, 5 * lvlDifficulty), random.Next(0, 5 * lvlDifficulty), r.size);
            }
        }
    }

    public Room getSpawn()
    {
        for (int i = 0; i != rooms.Count; i += 1)
        {
            if (rooms[i].isSpawn == true)
                return rooms[i];
        }
        return null;
    }

    public Room getEnd()
    {
        for (int i = 0; i != rooms.Count; i += 1)
        {
            if (rooms[i].isEnd == true)
                return rooms[i];
        }
        return null;
    }
}
