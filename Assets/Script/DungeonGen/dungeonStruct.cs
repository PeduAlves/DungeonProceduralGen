// using UnityEngine;
// using System.Collections.Generic;

// public enum DungeonType {

//     Florest,
//     Castle,
//     Random
// }
// public enum DungeonFloorType{

//     FloorFlorest,
//     GraveFlorest,
//     CrystalFlorest,
//     Boss,
//     Room,
//     Maze,
//     Arena,
//     Tesoure,
//     Haunted,
//     Elemental,
//     Magic,
//     Living
// }
// public enum DungeonRoomType {

//     Room,
//     Hall,
// }

// public class DungeonRoom {

//     public DungeonRoomType roomType;
//     public int roomId;
//     public Vector3Int size;
// }

// public class DungeonFloor {

//     public DungeonFloorType floorType;
//     public int floorId;
//     public Vector3Int size;
//     public List<DungeonRoom> rooms = new List<DungeonRoom>();
// }

// public class Dungeon {

//     public DungeonType dungeonType;
//     public List<DungeonFloor> floors = new List<DungeonFloor>();
// }