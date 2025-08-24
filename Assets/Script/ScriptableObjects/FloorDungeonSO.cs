using UnityEngine;

[CreateAssetMenu(fileName = "FloorDungeonSO", menuName = "Dungeon/FloorType")]
public class FloorDungeonSO : ScriptableObject {
    public string floorName;
    [TextArea] public string description;
    public float spawnProbability;
    public Vector3Int minSize, maxSize;
    public DungeonSo[] allowedDungeonTypes;
}


[System.Serializable]
public class RoomEntry {
    public RoomDungeonSO room;
    public float weight = 1f;
    public int minCount = 0; // mínimo de ocorrências desta room neste floor
    public int maxCount = 999; // máximo de ocorrências desta room neste floor
}