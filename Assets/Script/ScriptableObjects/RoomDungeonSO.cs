using UnityEngine;

[CreateAssetMenu(fileName = "RoomDungeonSO", menuName = "Dungeon/RoomType")]
public class RoomDungeonSO : ScriptableObject {

    public string roomName;
    [TextArea] public string description;
    public float spawnProbability;
    public Vector3Int minSize, maxSize;
    // public RoomGeneratorType generator;
    public FloorDungeonSO[] allowedFloors;
}
