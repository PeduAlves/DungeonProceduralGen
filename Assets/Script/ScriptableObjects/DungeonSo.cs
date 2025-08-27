using UnityEngine;

[CreateAssetMenu(fileName = "DungeonSo", menuName = "Dungeon/TypeDungeon")]
public class DungeonSO : ScriptableObject {
    public string dungeonName;
    [TextArea] public string description;
    public float spawnProbability;
    public int minFloors, maxFloors;
}

[System.Serializable]
public class FloorEntry {
    public FloorDungeonSO floor;
    public float weight = 1f;
    public int minCount = 0; // mínimo de ocorrências deste floor no dungeon
    public int maxCount = 999; // máximo de ocorrências deste floor no dungeon
}
