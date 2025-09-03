using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DungeonInstance
{
    public DungeonSO dungeonType;
    public List<FloorInstance> floors = new List<FloorInstance>();
    public int seed;
    public Vector3Int totalSize;  
    public DungeonInstance(DungeonSO type, int seed)
    {
        this.dungeonType = type;
        this.seed = seed;
    }
}

[System.Serializable]
public class FloorInstance
{
    public FloorDungeonSO floorType;
    public List<RoomInstance> rooms = new List<RoomInstance>();
    public Vector3Int position;
    public Vector3Int size;
    public int floorIndex;

    public BSPNode bspRoot;
    public FloorLayoutType layoutType;

    public FloorInstance(FloorDungeonSO type, int index)
    {
        this.floorType = type;
        this.floorIndex = index;
    }
}

[System.Serializable]
public class RoomInstance
{
    public RoomDungeonSO roomType;
    public RectInt bounds2D;
    public Vector3Int worldPosition;
    public Vector3Int size;
    public int roomId;

 
    public List<ConnectionPoint> connections = new List<ConnectionPoint>();
    public bool hasStairs;           
    public RoomInstance(RoomDungeonSO type, RectInt bounds)
    {
        this.roomType = type;
        this.bounds2D = bounds;
    }
}

[System.Serializable]
public class ConnectionPoint
{
    public Vector2Int position;
    public ConnectionType type;
    public int targetRoomId;

    public ConnectionPoint(Vector2Int pos, ConnectionType connType)
    {
        this.position = pos;
        this.type = connType;
        this.targetRoomId = -1;
    }
}

public enum FloorLayoutType 
{ 
    BSP,           
    SingleRoom,
    Grid,
    Organic
}

public enum ConnectionType 
{ 
    Door,
    Corridor,
    Stair,
    SecretDoor
}