using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class Room: MonoBehaviour
{
    public BoundsInt bounds;
    public Vector3Int location;
    public Vector3Int size;
    public GameObject go;

    public bool isSpawn = false;
    public bool isEnd = false;

    private EnemySpawner[] _spawners;

    private void Awake() => _spawners = gameObject.GetComponentsInChildren<EnemySpawner>(true);

    public Room(Vector3Int location, Vector3Int size)
    {
        bounds = new BoundsInt(location, size);
        this.size = size;
        this.location = location;
    }

    public void SetLocation(Vector3Int location)
    {
        bounds = new BoundsInt(location, size);
        this.location = location;
    }

    public static bool Intersect(Room a, Room b)
    {
        return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
            || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y)
            || (a.bounds.position.z >= (b.bounds.position.z + b.bounds.size.z)) || ((a.bounds.position.z + a.bounds.size.z) <= b.bounds.position.z));
    }

    public Room PlaceRoom(GameObject prefab)
    {
        Vector3Int realLocation = new Vector3Int(location.x * 12, location.y * 12, location.z * 12);
        Vector3 p = new Vector3(realLocation.x - 6f, realLocation.y - 5.999f, realLocation.z - 6f);
        go = Instantiate(prefab, p, Quaternion.identity);
        
        go.GetComponent<Transform>().localScale = new Vector3Int(1,1,1);

        Room r = go.GetComponent<Room>();
        r.bounds = new BoundsInt(location, size);
        r.size = size;
        r.location = location;
        r.go = go;
        return r;
    }
}
